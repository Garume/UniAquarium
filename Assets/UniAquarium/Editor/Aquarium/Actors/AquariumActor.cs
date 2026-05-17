using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Actors
{
    public abstract class AquariumActor : Actor<AquariumActor, AquariumSceneOption>
    {
        protected AquariumActor(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        public override void Initialize()
        {
            CreateNodes();
            base.Initialize();
        }

        protected abstract void CreateNodes();
    }
}
