using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class JellyFish : AquariumActor
    {
        private readonly Color _color;

        public JellyFish(Color color, AquariumSceneOption sceneOption) : base(sceneOption)
        {
            _color = color;
        }

        protected override void CreateNodes()
        {
            AddNode(new RenderNode<AquariumSceneOption>(new JellyFishShape(_color)));
            AddNode(new TargetTrackingNode(0.01f));
        }
    }
}
