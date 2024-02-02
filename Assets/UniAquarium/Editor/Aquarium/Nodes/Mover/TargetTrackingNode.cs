using System;
using System.Collections.Generic;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UniAquarium.Foundation;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium.Nodes
{
    internal class TargetTrackingReceivedData
    {
        public Action OnArrived;
        public float Speed;
        public Vector2 TargetPosition;
    }

    internal sealed class TargetTrackingNode : Node<AquariumSceneOption>
    {
        private readonly float _noiseSize;
        private readonly List<ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>> _receiverNodes = new();
        private readonly float _smoothCurveRate;
        private readonly float _speed;

        private float _actualSpeed;
        private float _angle;
        private bool _isForceTracking;
        private float _lastDeltaTime;
        private Action _onArrived;

        public TargetTrackingNode(float speed = 1, float noiseSize = 1f, float smoothCurveRate = 0.1f)
        {
            _speed = speed;
            _noiseSize = noiseSize;
            _smoothCurveRate = smoothCurveRate;
        }

        public bool HasTarget => TargetPosition != Vector2.zero;

        public bool AutoTarget { get; set; } = true;

        private float SpeedBias => _speed * 125f;

        public Vector2 TargetPosition { get; private set; } = Vector2.zero;

        public override void Draw(Painter2D painter, float deltaTime)
        {
            if (SceneOption.IsDebug)
            {
                var from = Transform.Position;
                var to = from + new Vector2(Mathf.Cos(Transform.Rotation), Mathf.Sin(Transform.Rotation)) * 20f;

                painter.fillColor = new Color(1f, 1f, 1f, 0.1f);
                painter.strokeColor = new Color(1f, 1f, 1f, 0.1f);
                painter.lineWidth = 1f;

                painter.FillCircle(from.x, from.y, 5f);

                painter.BeginPath();
                painter.MoveTo(from);
                painter.LineTo(to);
                painter.Stroke();
            }
        }

        public override void Update(float deltaTime)
        {
            foreach (var receiverNode in _receiverNodes)
            {
                var item = receiverNode.ReceivedItem;
                if (item == null) continue;

                TargetPosition = item.TargetPosition;
                _actualSpeed = item.Speed * SpeedBias;
                _onArrived = item.OnArrived;

                _isForceTracking = true;
            }

            UpdatePosition(deltaTime);
            _lastDeltaTime = deltaTime;
        }

        public void AddReceiver(ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption> receiver)
        {
            _receiverNodes.Add(receiver);
        }

        private void UpdatePosition(float deltaTime)
        {
            if (TargetPosition == Vector2.zero)
            {
                if (!AutoTarget || _isForceTracking) return;
                TargetPosition = new Vector2(Random.Range(0, SceneOption.Width), Random.Range(0, SceneOption.Height));
                _actualSpeed = SpeedBias * (1f + Random.Range(0f, 1f) * 0.5f);
                return;
            }

            var diff = TargetPosition - Transform.Position;

            var angleDiff = Mathf.Atan2(diff.y, diff.x);
            _angle = Mathf.LerpAngle(_angle, angleDiff, _smoothCurveRate);

            var vector = diff.normalized;
            var velocity = vector * _actualSpeed + new Vector2(Noise(), Noise());

            Transform.Velocity = velocity * deltaTime;
            Transform.Position += Transform.Velocity;
            Transform.Rotation = _angle;

            if (Vector2.Distance(Transform.Position, TargetPosition) < 2f && AutoTarget)
            {
                TargetPosition = Vector2.zero;
                _isForceTracking = false;
                _onArrived?.Invoke();
                _onArrived = null;
            }
            else if (Vector2.Distance(Transform.Position, TargetPosition) < 2f && _isForceTracking)
            {
                _isForceTracking = false;
                _onArrived?.Invoke();
                _onArrived = null;
            }

            return;

            float Noise()
            {
                return Random.Range(0f, 1f) * 0.5f * _actualSpeed * (AutoTarget ? _noiseSize : 0);
            }
        }

        public void TranslateTargetPosition(Vector2 moveVector, float speed)
        {
            if (_isForceTracking) return;

            _actualSpeed = speed;

            if (!HasTarget)
                TargetPosition = moveVector;
            else
                TargetPosition += moveVector;

            UpdatePosition(_lastDeltaTime);
        }
    }
}