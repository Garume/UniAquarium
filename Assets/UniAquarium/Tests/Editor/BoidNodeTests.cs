using NUnit.Framework;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Tests.Editor
{
    public sealed class BoidNodeTests
    {
        [Test]
        public void Update_WithSingleTrackingNode_DoesNotWriteNaNTarget()
        {
            var scene = new AquariumScene();
            var option = new AquariumSceneOption(320f, 240f, scene);
            var boidTransform = new TestTransform { Position = Vector2.zero };
            var trackingTransform = new TestTransform { Position = new Vector2(10f, 0f) };
            var boid = new BoidNode();
            var tracking = new TargetTrackingNode();

            boid.Initialize(boidTransform, option);
            tracking.Initialize(trackingTransform, option);
            boid.AddTrackingNode(tracking);

            boid.Update(0.1f);

            Assert.That(float.IsNaN(tracking.TargetPosition.x), Is.False);
            Assert.That(float.IsNaN(tracking.TargetPosition.y), Is.False);
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
