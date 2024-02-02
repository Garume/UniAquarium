using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium
{
    [Serializable]
    internal class FishSetting
    {
        [SerializeField] private FishType _fishType;
        [SerializeField] private float _angle;
        [SerializeField] private Color _color;
        [SerializeField] private Vector2 _location;
        [SerializeField] private float _scale;


        public FishType FishType
        {
            get => _fishType;
            set => _fishType = value;
        }

        public float Angle
        {
            get => _angle;
            set => _angle = value;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        public Vector2 Location
        {
            get => _location;
            set => _location = value;
        }

        public float Scale
        {
            get => _scale;
            set => _scale = value;
        }
    }

    [Serializable]
    internal class FishGroupSetting
    {
        [SerializeField] private FishSetting[] _fishSettings;

        public FishSetting[] FishSettings
        {
            get => _fishSettings;
            set => _fishSettings = value;
        }
    }

    [Serializable]
    internal class AquariumSetting
    {
        [SerializeField] private bool _canClean;
        [SerializeField] private bool _canFeed;

        [SerializeField] private FishGroupSetting[] _fishGroupSettings;

        public bool CanClean
        {
            get => _canClean;
            set => _canClean = value;
        }

        public bool CanFeed
        {
            get => _canFeed;
            set => _canFeed = value;
        }

        public FishGroupSetting[] FishGroupSettings
        {
            get => _fishGroupSettings;
            set => _fishGroupSettings = value;
        }
    }


    internal enum FishType
    {
        Fish,
        JellyFish,
        Lophophorata
    }

    internal sealed class UniAquariumSettings : ScriptableObject
    {
        private static UniAquariumSettings _instance;

        [SerializeField] private AquariumSetting _aquariumSetting;

        public static UniAquariumSettings Instance
        {
            get
            {
                var asset = PlayerSettings.GetPreloadedAssets().OfType<UniAquariumSettings>()
                    .FirstOrDefault();
                _instance = asset != null ? asset : CreateInstance<UniAquariumSettings>();
                return _instance;
            }
        }

        public AquariumSetting AquariumSetting
        {
            get => _aquariumSetting ?? GetDefaultSetting();
            private set => _aquariumSetting = value;
        }

        [MenuItem("Assets/Create/UniAquarium Settings", priority = -1)]
        private static void Create()
        {
            var asset = PlayerSettings.GetPreloadedAssets().OfType<UniAquariumSettings>().FirstOrDefault();
            if (asset != null)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                throw new InvalidOperationException($"{nameof(UniAquariumSettings)} already exists at {path}");
            }

            var assetPath = EditorUtility.SaveFilePanelInProject($"Save {nameof(UniAquariumSettings)}",
                nameof(UniAquariumSettings),
                "asset", "", "Assets");

            if (string.IsNullOrEmpty(assetPath)) return;

            var folderPath = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var instance = CreateInstance<UniAquariumSettings>();
            instance.AquariumSetting = GetDefaultSetting();
            AssetDatabase.CreateAsset(instance, assetPath);
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.Add(instance);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            AssetDatabase.SaveAssets();
        }

        private static AquariumSetting GetDefaultSetting()
        {
            var fishSettings = new List<FishGroupSetting>();

            for (var i = 0; i < 10; i++)
            {
                var setting = new List<FishSetting>();
                var color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
                for (var j = 0; j < Random.Range(5, 7); j++)
                    setting.Add(new FishSetting
                    {
                        FishType = FishType.Fish,
                        Angle = 0,
                        Color = color,
                        Location = new Vector2(0, 0),
                        Scale = 1
                    });

                var fishGroupSetting = new FishGroupSetting
                {
                    FishSettings = setting.ToArray()
                };

                fishSettings.Add(fishGroupSetting);
            }

            for (var i = 0; i < 10; i++)
            {
                var setting = new List<FishSetting>();
                var location = new Vector2(Random.Range(100f, 600f), Random.Range(100f, 600f));
                setting.Add(new FishSetting
                {
                    FishType = FishType.JellyFish,
                    Angle = 0,
                    Color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f),
                    Location = location,
                    Scale = 1
                });

                var jellyFishGroupSetting = new FishGroupSetting
                {
                    FishSettings = setting.ToArray()
                };

                fishSettings.Add(jellyFishGroupSetting);
            }

            var lophosporateSetting = new List<FishSetting>
            {
                new()
                {
                    FishType = FishType.Lophophorata,
                    Angle = 0,
                    Color = HexToColor("#3F51B5"),
                    Location = new Vector2(120, 200),
                    Scale = 1
                }
            };

            var lophosporateFishGroupSetting = new FishGroupSetting
            {
                FishSettings = lophosporateSetting.ToArray()
            };

            fishSettings.Add(lophosporateFishGroupSetting);


            return new AquariumSetting
            {
                FishGroupSettings = fishSettings.ToArray(),
                CanClean = true,
                CanFeed = true
            };
        }

        private static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");

            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            byte a = 255;

            if (hex.Length == 8) 
                a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
    }
}