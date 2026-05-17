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
            var trackingCount = _trackingNodes.Count;
            if (trackingCount == 0) return;

            var totalPosition = Vector2.zero;
            var totalVelocity = Transform.Velocity;
            for (var i = 0; i < trackingCount; i++)
            {
                var transform = _trackingNodes[i].Transform;
                totalPosition += transform.Position;
                totalVelocity += transform.Velocity;
            }

            for (var i = 0; i < trackingCount; i++)
            {
                var trackingNode = _trackingNodes[i];
                var moveVector = GetMovementVector(trackingNode, totalPosition, totalVelocity, trackingCount);
                var targetPosition = trackingNode.HasTarget ? moveVector + trackingNode.TargetPosition : moveVector;
                var diff = trackingNode.Transform.Position - targetPosition;
                var distance = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
                trackingNode.TranslateTargetPosition(moveVector, distance);
            }
        }

        private Vector2 GetMovementVector(
            TargetTrackingNode trackingNode,
            Vector2 totalPosition,
            Vector2 totalVelocity,
            int trackingCount)
        {
            var vector = Vector2.zero;

            CalculateVectorToCenter(trackingNode, totalPosition, trackingCount, ref vector);
            CalculateVectorToAvoid(trackingNode, ref vector);
            CalculateVectorToAlign(trackingNode, totalVelocity, trackingCount, ref vector);

            return vector;
        }

        private void CalculateVectorToCenter(
            TargetTrackingNode trackingNode,
            Vector2 totalPosition,
            int trackingCount,
            ref Vector2 result)
        {
            var position = trackingNode.Transform.Position;
            var otherCount = trackingCount - 1;
            var vector = otherCount == 0 ? Transform.Position : (totalPosition - position) / otherCount;

            vector += Transform.Position;
            vector /= 2;

            result += Normalize(vector - position) * _cohesion;
        }

        private void CalculateVectorToAvoid(TargetTrackingNode trackingNode, ref Vector2 result)
        {
            var vector = Vector2.zero;
            var thresholdSqr = _avoidThresholdDistance * _avoidThresholdDistance;
            var trackingPosition = trackingNode.Transform.Position;
            for (var i = 0; i < _trackingNodes.Count; i++)
            {
                var node = _trackingNodes[i];
                if (ReferenceEquals(node, trackingNode)) continue;

                var diff = node.Transform.Position - trackingPosition;
                if (diff.sqrMagnitude < thresholdSqr)
                    vector -= diff;
            }

            var boidDiff = Transform.Position - trackingPosition;
            if (boidDiff.sqrMagnitude < thresholdSqr)
                vector -= boidDiff;

            result += Normalize(vector) * _separation;
        }

        private void CalculateVectorToAlign(
            TargetTrackingNode trackingNode,
            Vector2 totalVelocity,
            int trackingCount,
            ref Vector2 result)
        {
            var vector = totalVelocity - trackingNode.Transform.Velocity;
            vector /= trackingCount;

            result += Normalize(vector) * _alignment;
        }

        private static Vector2 Normalize(Vector2 vector)
        {
            var sqrMagnitude = vector.x * vector.x + vector.y * vector.y;
            if (sqrMagnitude < 0.000001f) return Vector2.zero;

            var multiplier = 1f / Mathf.Sqrt(sqrMagnitude);
            return new Vector2(vector.x * multiplier, vector.y * multiplier);
        }

        public void AddTrackingNode(TargetTrackingNode targetTrackingNode)
        {
            if (targetTrackingNode == null) return;

            _trackingNodes.Add(targetTrackingNode);
            targetTrackingNode.AutoTarget = false;
        }
    }
}
