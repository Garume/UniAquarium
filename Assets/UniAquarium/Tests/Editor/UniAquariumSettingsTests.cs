using NUnit.Framework;
using System;
using UniAquarium.Aquarium;
using UniAquarium.Aquarium.Scene;
using UnityEngine;

namespace UniAquarium.Tests.Editor
{
    public sealed class UniAquariumSettingsTests
    {
        [Test]
        public void FishGroupSetting_ReturnsEmptyArrayWhenFishSettingsAreNull()
        {
            var setting = new FishGroupSetting();

            Assert.That(setting.FishSettings, Is.Empty);
        }

        [Test]
        public void AquariumSetting_ReturnsEmptyArrayWhenFishGroupSettingsAreNull()
        {
            var setting = new AquariumSetting();

            Assert.That(setting.FishGroupSettings, Is.Empty);
        }

        [Test]
        public void AquariumSetting_DefaultValueIsCached()
        {
            var settings = ScriptableObject.CreateInstance<UniAquariumSettings>();

            var first = settings.AquariumSetting;
            var second = settings.AquariumSetting;

            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void AquariumSceneOption_ResizeUpdatesViewport()
        {
            var scene = new AquariumScene();
            var option = new AquariumSceneOption(320f, 240f, scene);

            option.Resize(640f, 480f);

            Assert.That(option.Width, Is.EqualTo(640f));
            Assert.That(option.Height, Is.EqualTo(480f));
        }

        [Test]
        public void AquariumSceneOption_WhenSceneIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AquariumSceneOption(320f, 240f, null));
        }
    }
}
