using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class FoodSpawner : AquariumActor
    {
        public FoodSpawner(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        protected override void Configure(ActorBuilder builder)
        {
            builder.AddNode(new FoodSpawnerNode());
        }
    }
}
