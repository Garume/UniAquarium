using System.Collections.Generic;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine.UIElements;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class FoodSpawner : AquariumActor
    {
        public FoodSpawner(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        protected override IEnumerable<INode> CreateNodes()
        {
            return new INode[]
            {
                new FoodSpawnerNode()
            };
        }
    }
}