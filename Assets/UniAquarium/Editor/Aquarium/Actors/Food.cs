using System.Collections.Generic;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class Food : AquariumActor
    {
        public Food(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        public INode TargetNode { get; set; }

        protected override IEnumerable<INode> CreateNodes()
        {
            return new INode[]
            {
                new RenderNode<AquariumSceneOption>(new FoodShape()),
                new SwayFallingNode(12f, 4f)
            };
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Position.y > SceneOption.Height) Destroy();
        }
    }
}