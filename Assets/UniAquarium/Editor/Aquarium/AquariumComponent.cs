using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Components;
using UniAquarium.Foundation;
using UnityEngine;

namespace UniAquarium.Aquarium
{
    public class AquariumComponent : CanvasSceneComponent<AquariumScene, AquariumActor, AquariumSceneOption>
    {
        public AquariumComponent(bool interactive = true) : base(interactive)
        {
        }

        protected override float DeltaTime => EditorDeltaTime.DeltaTime * SceneOption.TimeScale;

        protected override void Initialize(AquariumSceneOption sceneOption)
        {
            var settings = UniAquariumSettings.Instance.AquariumSetting;

            if (settings.CanFeed) new FoodSpawner(sceneOption).Instantiate(Vector2.zero);
            if (settings.CanClean) new ShockwaveSpawner(sceneOption).Instantiate(Vector2.zero);

            foreach (var fishGroupSettings in settings.FishGroupSettings)
            {
                var fishSettings = fishGroupSettings.FishSettings;

                if (fishSettings.Length == 0) continue;
                if (fishSettings.Length == 1)
                {
                    FishFactory.Create(fishSettings[0], sceneOption).Instantiate(fishSettings[0].Location,
                        fishSettings[0].Angle, fishSettings[0].Scale);
                }
                else
                {
                    var boid = new Boid(sceneOption);
                    boid.Instantiate(Vector2.zero);
                    foreach (var fishSetting in fishSettings)
                    {
                        var fish = FishFactory.Create(fishSetting, sceneOption);
                        fish.Instantiate(fishSetting.Location, fishSetting.Angle, fishSetting.Scale);
                        boid.AddTrackingNode(fish.GetNode<TargetTrackingNode>());
                    }
                }
            }
        }

        protected override AquariumScene CreateScene()
        {
            return new AquariumScene();
        }

        protected override AquariumSceneOption CreateSceneOption(AquariumScene scene)
        {
            var width = resolvedStyle.width;
            var height = resolvedStyle.height;
            return new AquariumSceneOption(width, height, scene);
        }
    }
}