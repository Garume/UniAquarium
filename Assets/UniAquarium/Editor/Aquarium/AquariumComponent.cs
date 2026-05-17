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

        protected override float DeltaTime => EditorDeltaTime.DeltaTime * (SceneOption?.TimeScale ?? 1f);

        protected override void Initialize(AquariumSceneOption sceneOption)
        {
            var settings = UniAquariumSettings.Instance.AquariumSetting;

            if (settings.CanFeed) sceneOption.Utility.Spawn(new FoodSpawner(sceneOption), Vector2.zero, 0f, 1f);
            if (settings.CanClean) sceneOption.Utility.Spawn(new ShockwaveSpawner(sceneOption), Vector2.zero, 0f, 1f);

            foreach (var fishGroupSettings in settings.FishGroupSettings)
            {
                if (fishGroupSettings == null) continue;

                var fishSettings = fishGroupSettings.FishSettings;

                if (fishSettings.Length == 0) continue;
                if (fishSettings.Length == 1)
                {
                    if (fishSettings[0] == null) continue;

                    sceneOption.Utility.Spawn(FishFactory.Create(fishSettings[0], sceneOption),
                        fishSettings[0].Location, fishSettings[0].Angle, fishSettings[0].Scale);
                }
                else
                {
                    var boid = new Boid(sceneOption);
                    sceneOption.Utility.Spawn(boid, Vector2.zero, 0f, 1f);
                    foreach (var fishSetting in fishSettings)
                    {
                        if (fishSetting == null) continue;

                        var fish = FishFactory.Create(fishSetting, sceneOption);
                        sceneOption.Utility.Spawn(fish, fishSetting.Location, fishSetting.Angle,
                            fishSetting.Scale);
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

        protected override void ResizeSceneOption(AquariumSceneOption sceneOption, float width, float height)
        {
            sceneOption.Resize(width, height);
        }
    }
}
