using System.Collections.Generic;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UniAquarium.Foundation;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class BoidNode : Node<AquariumSceneOption>
    {
        // 群れの平均速度に合わせる度合い
        private readonly float _alignment;

        private readonly float _avoidThresholdDistance;

        // 群れの中心に向かう度合い
        private readonly float _cohesion;

        // 仲間を避ける度合い
        private readonly float _separation;
        private readonly List<TargetTrackingNode> _trackingNodes = new();

        public BoidNode(float cohesion = 8f, float separation = 16f, float alignment = 2f,
            float avoidThresholdDistance = 30f)
        {
            _cohesion = cohesion;
            _separation = separation;
            _alignment = alignment;
            _avoidThresholdDistance = avoidThresholdDistance;
        }

        public override void Draw(Painter2D painter, float deltaTime)
        {
            if (SceneOption.IsDebug)
            {
                painter.fillColor = new Color(1f, 0f, 0f, 0.3f);
                painter.FillCircle(Transform.Position.x, Transform.Position.y, 5f);
            }
        }

        public override void Update(float deltaTime)
        {
            foreach (var trackingNode in _trackingNodes)
            {
                var moveVector = GetMovementVector(trackingNode);
                var distance = Vector2.Distance(trackingNode.Transform.Position,
                    trackingNode.HasTarget ? moveVector + trackingNode.TargetPosition : moveVector);
                trackingNode.TranslateTargetPosition(moveVector, distance);
            }
        }

        private Vector2 GetMovementVector(TargetTrackingNode trackingNode)
        {
            var vector = Vector2.zero;

            CalculateVectorToCenter(trackingNode, ref vector);
            CalculateVectorToAvoid(trackingNode, ref vector);
            CalculateVectorToAlign(trackingNode, ref vector);

            return vector;
        }

        private void CalculateVectorToCenter(TargetTrackingNode trackingNode, ref Vector2 result)
        {
            var vector = Vector2.zero;
            var position = trackingNode.Transform.Position;

            foreach (var node in _trackingNodes)
            {
                if (node.Equals(trackingNode)) continue;
                vector += node.Transform.Position;
            }

            vector /= _trackingNodes.Count - 1;
            vector += Transform.Position;
            vector /= 2;

            result = (vector - position).normalized;
            result *= _cohesion;
        }

        private void CalculateVectorToAvoid(TargetTrackingNode trackingNode, ref Vector2 result)
        {
            var vector = Vector2.zero;
            foreach (var node in _trackingNodes)
            {
                if (node.Equals(trackingNode)) continue;
                if (Vector2.Distance(node.Transform.Position, trackingNode.Transform.Position) <
                    _avoidThresholdDistance)
                    vector -= node.Transform.Position - trackingNode.Transform.Position;
            }

            if (Vector2.Distance(Transform.Position, trackingNode.Transform.Position) <
                _avoidThresholdDistance)
                vector -= Transform.Position - trackingNode.Transform.Position;

            result += vector.normalized;
            result *= _separation;
        }

        private void CalculateVectorToAlign(TargetTrackingNode trackingNode, ref Vector2 result)
        {
            var vector = Vector2.zero;
            foreach (var node in _trackingNodes)
            {
                if (node.Equals(trackingNode)) continue;
                vector += node.Transform.Velocity;
            }

            vector += Transform.Velocity;
            vector /= _trackingNodes.Count;

            result += vector.normalized;
            result *= _alignment;
        }

        public void AddTrackingNode(TargetTrackingNode targetTrackingNode)
        {
            _trackingNodes.Add(targetTrackingNode);
            targetTrackingNode.AutoTarget = false;
        }
    }
}