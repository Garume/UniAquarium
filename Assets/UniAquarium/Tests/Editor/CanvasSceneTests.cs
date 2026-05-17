using System.Collections.Generic;
using System;
using NUnit.Framework;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Tests.Editor
{
    public sealed class CanvasSceneTests
    {
        [Test]
        public void Spawn_InitializesTransformAndAddsActor()
        {
            var scene = new TestScene();
            var option = new TestSceneOption(320f, 240f, scene);
            var actor = new TestActor(option);

            scene.Spawn(actor, new Vector2(12f, 34f), 1.5f, 2f);

            var actorBuffer = new List<TestActor>();
            Assert.That(scene.GetActors(actorBuffer), Is.EqualTo(1));
            Assert.That(actorBuffer[0], Is.SameAs(actor));
            Assert.That(actor.Position, Is.EqualTo(new Vector2(12f, 34f)));
            Assert.That(actor.Rotation, Is.EqualTo(1.5f));
            Assert.That(actor.Scale, Is.EqualTo(2f));
            Assert.That(actor.InitializeCount, Is.EqualTo(1));
        }

        [Test]
        public void Spawn_WhenActorIsAlreadySpawned_Throws()
        {
            var scene = new TestScene();
            var option = new TestSceneOption(320f, 240f, scene);
            var actor = new TestActor(option);

            scene.Spawn(actor, Vector2.zero, 0f, 1f);

            Assert.Throws<InvalidOperationException>(() => scene.Spawn(actor, Vector2.zero, 0f, 1f));
            Assert.That(actor.InitializeCount, Is.EqualTo(1));
        }

        [Test]
        public void Update_RemovesDestroyedActorsWithoutSkippingNextActor()
        {
            var scene = new TestScene();
            var option = new TestSceneOption(320f, 240f, scene);
            var actors = new[]
            {
                new TestActor(option) { DestroyOnUpdate = true },
                new TestActor(option) { DestroyOnUpdate = true },
                new TestActor(option) { DestroyOnUpdate = true }
            };

            foreach (var actor in actors)
                scene.Spawn(actor, Vector2.zero, 0f, 1f);

            scene.Update(0.1f);

            var actorBuffer = new List<TestActor>();
            Assert.That(scene.GetActors(actorBuffer), Is.EqualTo(0));
            for (var i = 0; i < actors.Length; i++)
                Assert.That(actors[i].UpdateCount, Is.EqualTo(1));
        }

        private sealed class TestScene : CanvasScene<TestActor>
        {
        }

        private sealed class TestSceneOption : ISceneOption<TestActor>
        {
            public TestSceneOption(float width, float height, ISceneUtility<TestActor> utility)
            {
                Width = width;
                Height = height;
                Utility = utility;
            }

            public float Width { get; }
            public float Height { get; }
            public ISceneUtility<TestActor> Utility { get; }
        }

        private sealed class TestActor : Actor<TestActor, TestSceneOption>
        {
            public TestActor(TestSceneOption sceneOption) : base(sceneOption)
            {
            }

            public bool DestroyOnUpdate { get; set; }
            public int InitializeCount { get; private set; }
            public int UpdateCount { get; private set; }

            public override void Initialize()
            {
                InitializeCount++;
                base.Initialize();
            }

            public override void Update(float deltaTime)
            {
                UpdateCount++;
                base.Update(deltaTime);

                if (DestroyOnUpdate) Destroy();
            }
        }
    }
}
