using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    public interface IPaintable
    {
        void Draw(Painter2D painter, float deltaTime);
        void Update(float deltaTime);
    }
}