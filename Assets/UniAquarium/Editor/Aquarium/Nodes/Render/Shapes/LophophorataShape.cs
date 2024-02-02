using UniAquarium.Core.Paints;
using UnityEngine;
using UnityEngine.UIElements;
using ITransform = UniAquarium.Core.Paints.ITransform;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium.Nodes
{
    internal class LophophorataShape : Shape
    {
        private readonly CompositeShape _compositeShape;

        public LophophorataShape(Color color, int branchCount = 10)
        {
            _compositeShape = new CompositeShape { new MarbleCircle(new[] { color }, 5f) };

            for (var i = 0; i < branchCount; i++)
                _compositeShape.Add(new BranchShape(color, Random.Range(0f, 360f), 0.01f));
        }

        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            _compositeShape.Draw(painter, transform, deltaTime);
        }
    }
}