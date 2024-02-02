using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal abstract class Shape
    {
        public abstract void Draw(Painter2D painter, ITransform transform, float deltaTime);
    }
}