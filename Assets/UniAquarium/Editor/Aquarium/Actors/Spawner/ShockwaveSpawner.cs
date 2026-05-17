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

        protected override void Configure(ActorBuilder builder)
        {
            builder.AddNode(new ShockwaveSpawnerNode());
        }
    }
}
