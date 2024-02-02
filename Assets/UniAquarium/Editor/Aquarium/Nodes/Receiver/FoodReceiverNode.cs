using System.Linq;
using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class FoodReceiverNode : ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>
    {
        private readonly float _speed;
        private readonly float _triggerAngle;
        private readonly float _triggerDistance;
        private FoodSpawnerNode _foodSpawner;

        public FoodReceiverNode(float triggerDistance = 200f, float triggerAngle = 160f, float speed = 3f)
        {
            _triggerDistance = triggerDistance;
            _triggerAngle = triggerAngle;
            _speed = speed;
        }

        protected override TargetTrackingReceivedData UpdateReceived()
        {
            if (_foodSpawner == null)
            {
                var foodSpawner = SceneOption.Utility.GetActor<FoodSpawner>();
                _foodSpawner = foodSpawner?.GetNode<FoodSpawnerNode>() as FoodSpawnerNode;

                if (_foodSpawner == null)
                    return null;
            }


            var filteredFoods = _foodSpawner.Actors.Where(t => Equals(t.TargetNode, this) || t.TargetNode == null)
                .ToList();

            if (filteredFoods.Count == 0)
                return null;


            var food = filteredFoods[0];
            foreach (var t in filteredFoods.Where(t => Vector2.Distance(Transform.Position, t.Position) <
                                                       Vector2.Distance(Transform.Position, food.Position)))
                food = t;

            if (Vector2.Distance(Transform.Position, food.Position) > _triggerDistance)
                return null;

            var angleDiff = Mathf.Atan2(food.Position.y - Transform.Position.y, food.Position.x - Transform.Position.x);
            var selfAngle = Mathf.Atan2(Transform.Velocity.y, Transform.Velocity.x);

            if (!(Mathf.Abs(selfAngle - angleDiff) < _triggerAngle * Mathf.Deg2Rad)) return null;
            food.TargetNode = this;

            var data = new TargetTrackingReceivedData
            {
                TargetPosition = food.Position,
                Speed = _speed,
                OnArrived = () => food.Destroy()
            };

            return data;
        }
    }
}