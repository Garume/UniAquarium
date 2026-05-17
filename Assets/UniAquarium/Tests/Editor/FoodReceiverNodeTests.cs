using NUnit.Framework;
using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Tests.Editor
{
    public sealed class FoodReceiverNodeTests
    {
        [Test]
        public void Update_ReceivesFoodFromSceneActors()
        {
            var scene = new AquariumScene();
            var option = new AquariumSceneOption(320f, 240f, scene);
            var transform = new TestTransform
            {
                Position = Vector2.zero,
                Velocity = Vector2.right
            };
            var receiver = new FoodReceiverNode();
            var food = new Food(option);

            scene.Spawn(food, new Vector2(20f, 0f), 0f, 1f);
            receiver.Initialize(transform, option);

            receiver.Update(0.1f);

            Assert.That(receiver.HasReceivedItem, Is.True);
            Assert.That(receiver.ReceivedItem.TargetPosition, Is.EqualTo(food.Position));
            Assert.That(food.TargetNode, Is.SameAs(receiver));
        }

        private sealed class TestTransform : ITransform
        {
            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
            public float Rotation { get; set; }
            public float Scale { get; set; }
        }
    }
}
