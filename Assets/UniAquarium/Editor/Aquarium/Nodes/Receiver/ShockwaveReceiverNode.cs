using System.Linq;
using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class ShockwaveReceiverNode : ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>
    {
        private readonly float _avoidDistance;
        private readonly float _triggerDistance;
        private ShockwaveSpawnerNode _shockwaveSpawner;

        public ShockwaveReceiverNode(float triggerDistance = 40f, float avoidDistance = 80f)
        {
            _triggerDistance = triggerDistance;
            _avoidDistance = avoidDistance;
        }

        protected override TargetTrackingReceivedData UpdateReceived()
        {
            if (_shockwaveSpawner == null)
            {
                var shockwaveSpawner = SceneOption.Utility.GetActor<ShockwaveSpawner>();
                _shockwaveSpawner = shockwaveSpawner?.GetNode<ShockwaveSpawnerNode>() as ShockwaveSpawnerNode;

                if (_shockwaveSpawner == null)
                    return null;
            }

            var shockwave = _shockwaveSpawner.Actors.FirstOrDefault(x =>
                Vector2.Distance(x.Position, Transform.Position) < _triggerDistance);

            if (shockwave == null)
                return null;

            var vector = (shockwave.Position - Transform.Position).normalized;
            var toX = Transform.Position.x - vector.x * _avoidDistance * Random.Range(0f, 1f);
            var toY = Transform.Position.y - vector.y * _avoidDistance * Random.Range(0f, 1f);
            var to = new Vector2(toX, toY);

            var targetSpeed = Random.Range(6f, 10f);
            var lastSpeed = Mathf.Lerp(200f, 300f, Random.Range(0f, 1f));

            var data = new TargetTrackingReceivedData
            {
                TargetPosition = to,
                Speed = targetSpeed,
                OnArrived = () => Transform.Velocity = Transform.Velocity.normalized * lastSpeed
            };

            return data;
        }
    }
}