using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class Fish : AquariumActor
    {
        private readonly Color _color;

        public Fish(Color color, AquariumSceneOption sceneOption) : base(sceneOption)
        {
            _color = color;
        }

        protected override void CreateNodes()
        {
            var targetTrackingNode = new TargetTrackingNode(0.1f);
            var shockwaveReceiverNode = new ShockwaveReceiverNode();
            var foodReceiverNode = new FoodReceiverNode();
            targetTrackingNode.AddReceiver(shockwaveReceiverNode);
            targetTrackingNode.AddReceiver(foodReceiverNode);

            AddNode(new RenderNode<AquariumSceneOption>(new FishShape(_color)));
            AddNode(targetTrackingNode);
            AddNode(foodReceiverNode);
            AddNode(shockwaveReceiverNode);
        }
    }
}
