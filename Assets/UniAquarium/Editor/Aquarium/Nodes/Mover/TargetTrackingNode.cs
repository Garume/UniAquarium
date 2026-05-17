using System.Collections.Generic;
using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UniAquarium.Foundation;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium.Nodes
{
    internal enum TargetTrackingArrivalAction
    {
        None,
        DestroyFood,
        ApplyVelocity
    }

    internal struct TargetTrackingReceivedData
    {
        public TargetTrackingArrivalAction ArrivalAction;
        public Food Food;
        public UniAquarium.Core.Paints.ITransform Transform;
        public float LastSpeed;
        public float Speed;
        public Vector2 TargetPosition;

        public void InvokeArrival()
        {
            switch (ArrivalAction)
            {
                case TargetTrackingArrivalAction.DestroyFood:
                    Food?.Destroy();
                    break;
                case TargetTrackingArrivalAction.ApplyVelocity:
                    Transform.Velocity = Normalize(Transform.Velocity) * LastSpeed;
                    break;
            }
        }

        private static Vector2 Normalize(Vector2 vector)
        {
            var sqrMagnitude = vector.x * vector.x + vector.y * vector.y;
            if (sqrMagnitude < 0.000001f) return Vector2.zero;

            var multiplier = 1f / Mathf.Sqrt(sqrMagnitude);
            return new Vector2(vector.x * multiplier, vector.y * multiplier);
        }
    }

    internal sealed class TargetTrackingNode : Node<AquariumSceneOption>
    {
        private readonly float _noiseSize;
        private readonly List<ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>> _receiverNodes = new();
        private readonly float _smoothCurveRate;
        private readonly float _speed;

        private float _actualSpeed;
        private float _angle;
        private bool _hasTarget;
        private bool _isForceTracking;
        private float _lastDeltaTime;
        private TargetTrackingReceivedData _receivedData;

        public TargetTrackingNode(float speed = 1, float noiseSize = 1f, float smoothCurveRate = 0.1f)
        {
            _speed = speed;
            _noiseSize = noiseSize;
            _smoothCurveRate = smoothCurveRate;
        }

        public bool HasTarget => _hasTarget;

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
            for (var i = 0; i < _receiverNodes.Count; i++)
            {
                var receiverNode = _receiverNodes[i];
                if (!receiverNode.HasReceivedItem) continue;

                var item = receiverNode.ReceivedItem;
                TargetPosition = item.TargetPosition;
                _hasTarget = true;
                _actualSpeed = item.Speed * SpeedBias;
                _receivedData = item;

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
            if (!_hasTarget)
            {
                if (!AutoTarget || _isForceTracking) return;
                TargetPosition = new Vector2(Random.Range(0, SceneOption.Width), Random.Range(0, SceneOption.Height));
                _hasTarget = true;
                _actualSpeed = SpeedBias * (1f + Random.Range(0f, 1f) * 0.5f);
                return;
            }

            var diff = TargetPosition - Transform.Position;

            var angleDiff = Mathf.Atan2(diff.y, diff.x);
            _angle = Mathf.LerpAngle(_angle, angleDiff, _smoothCurveRate);

            var vector = Normalize(diff);
            var noiseScale = AutoTarget ? _noiseSize : 0f;
            var noise = 0.5f * _actualSpeed * noiseScale;
            var velocity = vector * _actualSpeed + new Vector2(
                Random.Range(0f, 1f) * noise,
                Random.Range(0f, 1f) * noise
            );

            Transform.Velocity = velocity * deltaTime;
            Transform.Position += Transform.Velocity;
            Transform.Rotation = _angle;

            var arrived = (Transform.Position - TargetPosition).sqrMagnitude < 4f;
            if (arrived && AutoTarget)
            {
                TargetPosition = Vector2.zero;
                _hasTarget = false;
                _isForceTracking = false;
                _receivedData.InvokeArrival();
                _receivedData = default;
            }
            else if (arrived && _isForceTracking)
            {
                TargetPosition = Vector2.zero;
                _hasTarget = false;
                _isForceTracking = false;
                _receivedData.InvokeArrival();
                _receivedData = default;
            }

        }

        public void TranslateTargetPosition(Vector2 moveVector, float speed)
        {
            if (_isForceTracking) return;

            _actualSpeed = speed;

            if (!HasTarget)
            {
                TargetPosition = moveVector;
                _hasTarget = true;
            }
            else
            {
                TargetPosition += moveVector;
            }

            UpdatePosition(_lastDeltaTime);
        }

        private static Vector2 Normalize(Vector2 vector)
        {
            var sqrMagnitude = vector.x * vector.x + vector.y * vector.y;
            if (sqrMagnitude < 0.000001f) return Vector2.zero;

            var multiplier = 1f / Mathf.Sqrt(sqrMagnitude);
            return new Vector2(vector.x * multiplier, vector.y * multiplier);
        }
    }
}
