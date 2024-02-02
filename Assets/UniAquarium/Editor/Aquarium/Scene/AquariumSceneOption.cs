using UniAquarium.Aquarium.Actors;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Scene
{
    public sealed class AquariumSceneOption : ISceneOption
    {
        public AquariumSceneOption(float width, float height, ISceneUtility<AquariumActor> utility,
            bool isDebug = false)
        {
            Width = width;
            Height = height;
            Utility = utility;
            IsDebug = isDebug;
        }

        public bool IsDebug { get; set; }
        public float Width { get; }
        public float Height { get; }
        public ISceneUtility<AquariumActor> Utility { get; }
    }
}