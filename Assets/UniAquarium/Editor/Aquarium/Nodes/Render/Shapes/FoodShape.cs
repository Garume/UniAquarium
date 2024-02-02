using UniAquarium.Core.Paints;
using UnityEngine.UIElements;
using ITransform = UniAquarium.Core.Paints.ITransform;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class FoodShape : Shape
    {
        private readonly Shape _shape;

        public FoodShape(float size = 3f)
        {
            _shape = new MarbleCircle(new[]
            {
                Random.ColorHSV(),
                Random.ColorHSV(),
                Random.ColorHSV(),
                Random.ColorHSV()
            }, size);
        }

        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            _shape.Draw(painter, transform, deltaTime);
        }
    }
}