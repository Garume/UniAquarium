using UnityEngine;

namespace UniAquarium.Core.Paints
{
    public interface ITransform
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        float Rotation { get; set; }
        float Scale { get; set; }
    }
}