using System;
using UniAquarium.Aquarium.Scene;

namespace UniAquarium.Aquarium.Actors
{
    internal static class FishFactory
    {
        internal static AquariumActor Create(FishSetting fishSetting, AquariumSceneOption sceneOption)
        {
            if (fishSetting == null) throw new ArgumentNullException(nameof(fishSetting));

            return fishSetting.FishType switch
            {
                FishType.Fish => new Fish(fishSetting.Color, sceneOption),
                FishType.JellyFish => new JellyFish(fishSetting.Color, sceneOption),
                FishType.Lophophorata => new Lophophorata(fishSetting.Color, sceneOption),
                _ => throw new ArgumentOutOfRangeException(nameof(fishSetting.FishType), fishSetting.FishType, null)
            };
        }
    }
}
