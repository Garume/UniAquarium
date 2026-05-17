using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;

namespace UniAquarium.Aquarium.Actors
{
    internal class ShockwaveSpawner : AquariumActor
    {
        public ShockwaveSpawner(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        protected override void CreateNodes()
        {
            AddNode(new ShockwaveSpawnerNode());
        }
    }
}
