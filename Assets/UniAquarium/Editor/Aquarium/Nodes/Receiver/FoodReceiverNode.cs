using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class FoodReceiverNode : ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>
    {
        private readonly float _speed;
        private readonly float _triggerCos;
        private readonly float _triggerDistanceSqr;

        public FoodReceiverNode(float triggerDistance = 200f, float triggerAngle = 160f, float speed = 3f)
        {
            _triggerDistanceSqr = triggerDistance * triggerDistance;
            _triggerCos = Mathf.Cos(triggerAngle * Mathf.Deg2Rad);
            _speed = speed;
        }

        protected override bool TryUpdateReceived(out TargetTrackingReceivedData receivedItem)
        {
            var foods = SceneOption.Scene.Foods;
            var selfPosition = Transform.Position;
            Food nearestFood = null;
            var nearestDistanceSqr = float.MaxValue;

            for (var i = 0; i < foods.Count; i++)
            {
                var food = foods[i];
                if (food.IsDestroyed) continue;
                if (!ReferenceEquals(food.TargetNode, this) && food.TargetNode != null) continue;

                var distanceSqr = (selfPosition - food.Position).sqrMagnitude;
                if (distanceSqr >= nearestDistanceSqr) continue;

                nearestFood = food;
                nearestDistanceSqr = distanceSqr;
            }

            if (nearestFood == null || nearestDistanceSqr > _triggerDistanceSqr)
            {
                receivedItem = default;
                return false;
            }

            var velocity = Transform.Velocity;
            var velocitySqr = velocity.sqrMagnitude;
            if (velocitySqr > 0.0001f)
            {
                var toFood = nearestFood.Position - selfPosition;
                var dot = Vector2.Dot(velocity, toFood);
                var cos = dot / Mathf.Sqrt(velocitySqr * nearestDistanceSqr);
                if (cos < _triggerCos)
                {
                    receivedItem = default;
                    return false;
                }
            }

            nearestFood.TargetNode = this;

            receivedItem = new TargetTrackingReceivedData
            {
                TargetPosition = nearestFood.Position,
                Speed = _speed,
                ArrivalAction = TargetTrackingArrivalAction.DestroyFood,
                Food = nearestFood
            };
            return true;
        }
    }
}
