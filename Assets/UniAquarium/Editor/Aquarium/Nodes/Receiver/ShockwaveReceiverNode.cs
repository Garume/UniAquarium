using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class ShockwaveReceiverNode : ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>
    {
        private readonly float _avoidDistance;
        private readonly float _triggerDistanceSqr;

        public ShockwaveReceiverNode(float triggerDistance = 40f, float avoidDistance = 80f)
        {
            _triggerDistanceSqr = triggerDistance * triggerDistance;
            _avoidDistance = avoidDistance;
        }

        protected override bool TryUpdateReceived(out TargetTrackingReceivedData receivedItem)
        {
            var shockwaves = SceneOption.Scene.Shockwaves;
            var selfPosition = Transform.Position;
            Shockwave shockwave = null;

            for (var i = 0; i < shockwaves.Count; i++)
            {
                var candidate = shockwaves[i];
                if (candidate.IsDestroyed) continue;
                if ((candidate.Position - selfPosition).sqrMagnitude >= _triggerDistanceSqr) continue;

                shockwave = candidate;
                break;
            }

            if (shockwave == null)
            {
                receivedItem = default;
                return false;
            }

            var vector = Normalize(shockwave.Position - selfPosition);
            var toX = selfPosition.x - vector.x * _avoidDistance * Random.Range(0f, 1f);
            var toY = selfPosition.y - vector.y * _avoidDistance * Random.Range(0f, 1f);
            var to = new Vector2(toX, toY);

            var targetSpeed = Random.Range(6f, 10f);
            var lastSpeed = Mathf.Lerp(200f, 300f, Random.Range(0f, 1f));

            receivedItem = new TargetTrackingReceivedData
            {
                TargetPosition = to,
                Speed = targetSpeed,
                ArrivalAction = TargetTrackingArrivalAction.ApplyVelocity,
                Transform = Transform,
                LastSpeed = lastSpeed
            };
            return true;
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
