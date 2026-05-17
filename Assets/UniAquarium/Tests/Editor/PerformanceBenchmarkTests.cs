using NUnit.Framework;
using UniAquarium.Performance;

namespace UniAquarium.Tests.Editor
{
    public sealed class PerformanceBenchmarkTests
    {
        [Test]
        public void Run_CompletesUpdateBenchmark()
        {
            var result = UniAquariumPerformanceBenchmark.Run(
                groups: 2,
                fishPerGroup: 3,
                foods: 4,
                shockwaves: 2,
                frames: 10
            );

            Assert.That(result.Frames, Is.EqualTo(10));
            Assert.That(result.TotalMilliseconds, Is.GreaterThanOrEqualTo(0d));
            Assert.That(result.MillisecondsPerFrame, Is.GreaterThanOrEqualTo(0d));
            Assert.That(result.AllocatedBytes, Is.EqualTo(0));
        }

        [Test]
        public void RunManagedCore_CompletesWithoutUnityNativeRandom()
        {
            var result = UniAquariumPerformanceBenchmark.RunManagedCore(
                actors: 10,
                nodesPerActor: 3,
                frames: 10
            );

            Assert.That(result.Frames, Is.EqualTo(10));
            Assert.That(result.TotalMilliseconds, Is.GreaterThanOrEqualTo(0d));
            Assert.That(result.AllocatedBytes, Is.EqualTo(0));
        }
    }
}
