using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class FoodSpawner : AquariumActor
    {
        public FoodSpawner(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        protected override void CreateNodes()
        {
            AddNode(new FoodSpawnerNode());
        }
    }
}
