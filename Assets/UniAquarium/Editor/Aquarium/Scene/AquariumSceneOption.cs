using System;
using UniAquarium.Aquarium.Actors;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Scene
{
    public sealed class AquariumSceneOption : ISceneOption<AquariumActor>
    {
        public AquariumSceneOption(float width, float height, AquariumScene scene, bool isDebug = false)
        {
            if (scene == null) throw new ArgumentNullException(nameof(scene));

            Width = width;
            Height = height;
            Scene = scene;
            Utility = scene;
            IsDebug = isDebug;
        }

        internal AquariumScene Scene { get; }
        public bool IsDebug { get; set; }
        public float TimeScale { get; set; } = 1f;
        public float Width { get; private set; }
        public float Height { get; private set; }
        public ISceneUtility<AquariumActor> Utility { get; }

        public void Resize(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
}
