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

        [Test]
        public void CalculateHeadCurvePoints_StaysBetweenHeadEdges()
        {
            var shape = new JellyFishShape(Color.cyan);
            var left = new Vector2[10];
            var center = new Vector2[10];
            var right = new Vector2[10];

            shape.AdvanceCapPointAngles(1f / 60f);
            shape.CalculateHeadEdgePoints(1f, left, right);
            shape.CalculateHeadCurvePoints(1f, 0f, center);

            for (var i = 0; i < center.Length; i++)
            {
                Assert.That(center[i].x, Is.EqualTo(0f).Within(0.0001f));
                Assert.That(center[i].y, Is.EqualTo(left[i].y).Within(0.0001f));
                Assert.That(center[i].y, Is.EqualTo(right[i].y).Within(0.0001f));
                Assert.That(center[i].x, Is.GreaterThanOrEqualTo(right[i].x));
                Assert.That(center[i].x, Is.LessThanOrEqualTo(left[i].x));
            }
        }
    }
}
