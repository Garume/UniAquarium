using NUnit.Framework;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Tests.Editor
{
    public sealed class TargetTrackingNodeTests
    {
        [Test]
        public void Update_AcceptsZeroVectorAsValidForcedTarget()
        {
            var scene = new AquariumScene();
            var option = new AquariumSceneOption(320f, 240f, scene);
            var transform = new TestTransform { Position = new Vector2(10f, 0f) };
            var receiver = new StubReceiverNode();
            var tracking = new TargetTrackingNode { AutoTarget = false };

            receiver.Initialize(transform, option);
            tracking.Initialize(transform, option);
            tracking.AddReceiver(receiver);

            receiver.Update(0.1f);
            tracking.Update(0.1f);

            Assert.That(tracking.HasTarget, Is.True);
            Assert.That(tracking.TargetPosition, Is.EqualTo(Vector2.zero));
        }

        private sealed class StubReceiverNode : ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>
        {
            protected override bool TryUpdateReceived(out TargetTrackingReceivedData receivedItem)
            {
                receivedItem = new TargetTrackingReceivedData
                {
                    TargetPosition = Vector2.zero,
                    Speed = 0.01f
                };
                return true;
            }
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
