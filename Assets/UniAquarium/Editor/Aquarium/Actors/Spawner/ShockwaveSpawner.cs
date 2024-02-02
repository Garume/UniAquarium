using System.Collections.Generic;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Actors
{
    internal class ShockwaveSpawner : AquariumActor
    {
        public ShockwaveSpawner(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        protected override IEnumerable<INode> CreateNodes()
        {
            return new INode[]
            {
                new ShockwaveSpawnerNode()
            };
        }
    }
}