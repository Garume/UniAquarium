using System.Collections.Generic;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Actors
{
    public abstract class AquariumActor : Actor<AquariumSceneOption>
    {
        protected AquariumActor(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        public override void Initialize()
        {
            var nodes = CreateNodes();
            AddNodes(nodes);
            base.Initialize();
        }

        protected abstract IEnumerable<INode> CreateNodes();
    }
}