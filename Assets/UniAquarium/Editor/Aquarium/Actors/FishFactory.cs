using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Actors
{
    internal static class FishFactory
    {
        internal static IActor Create(FishSetting fishSetting, AquariumSceneOption sceneOption)
        {
            return fishSetting.FishType switch
            {
                FishType.Fish => new Fish(fishSetting.Color, sceneOption),
                FishType.JellyFish => new JellyFish(fishSetting.Color, sceneOption),
                FishType.Lophophorata => new Lophophorata(fishSetting.Color, sceneOption),
                _ => null
            };
        }
    }
}