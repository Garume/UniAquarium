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
            var receiver = new StubReceiverNode(0.01f);
            var tracking = new TargetTrackingNode { AutoTarget = false };

            receiver.Initialize(transform, option);
            tracking.Initialize(transform, option);
            tracking.AddReceiver(receiver);

            receiver.Update(0.1f);
            tracking.Update(0.1f);

            Assert.That(tracking.HasTarget, Is.True);
            Assert.That(tracking.TargetPosition, Is.EqualTo(Vector2.zero));
        }

        [Test]
        public void Update_WhenForcedTargetArrives_ClearsTarget()
        {
            var scene = new AquariumScene();
            var option = new AquariumSceneOption(320f, 240f, scene);
            var transform = new TestTransform { Position = Vector2.zero };
            var receiver = new StubReceiverNode(1f);
            var tracking = new TargetTrackingNode { AutoTarget = false };

            receiver.Initialize(transform, option);
            tracking.Initialize(transform, option);
            tracking.AddReceiver(receiver);

            receiver.Update(0.1f);
            tracking.Update(0.1f);

            Assert.That(tracking.HasTarget, Is.False);
        }

        private sealed class StubReceiverNode : ReceiverNode<TargetTrackingReceivedData, AquariumSceneOption>
        {
            private readonly float _speed;

            public StubReceiverNode(float speed)
            {
                _speed = speed;
            }

            protected override bool TryUpdateReceived(out TargetTrackingReceivedData receivedItem)
            {
                receivedItem = new TargetTrackingReceivedData
                {
                    TargetPosition = Vector2.zero,
                    Speed = _speed
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
