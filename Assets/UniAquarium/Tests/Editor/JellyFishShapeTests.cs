using NUnit.Framework;
using UniAquarium.Aquarium.Nodes;
using UnityEngine;

namespace UniAquarium.Tests.Editor
{
    public sealed class JellyFishShapeTests
    {
        [Test]
        public void CalculateHeadEdgePoints_ReturnsSymmetricNonNaNPoints()
        {
            var shape = new JellyFishShape(Color.cyan);
            var left = new Vector2[10];
            var right = new Vector2[10];

            shape.AdvanceCapPointAngles(1f / 60f);
            shape.CalculateHeadEdgePoints(1f, left, right);

            for (var i = 0; i < left.Length; i++)
            {
                Assert.That(float.IsNaN(left[i].x), Is.False);
                Assert.That(float.IsNaN(left[i].y), Is.False);
                Assert.That(float.IsNaN(right[i].x), Is.False);
                Assert.That(float.IsNaN(right[i].y), Is.False);
                Assert.That(left[i].x, Is.EqualTo(-right[i].x).Within(0.0001f));
                Assert.That(left[i].y, Is.EqualTo(right[i].y).Within(0.0001f));
            }
        }
    }
}
