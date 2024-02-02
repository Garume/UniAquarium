using UniAquarium.Core.Paints;
using UniAquarium.Foundation;
using UnityEngine;
using UnityEngine.UIElements;
using ITransform = UniAquarium.Core.Paints.ITransform;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class MarbleCircle : Shape
    {
        private readonly Color[] _colors;

        private readonly float _size;


        public MarbleCircle(Color[] colors, float size = 1f, float opacity = 1f)
        {
            _colors = colors;
            _size = size;
            Opacity = opacity;
        }

        public float Opacity { get; set; }

        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            var size = transform.Scale * _size;
            var bias = 1 / _colors.Length;
            foreach (var color in _colors)
            {
                var c = color;
                c.a = Opacity;
                painter.fillColor = c;
                painter.FillCircle(transform.Position.x, transform.Position.y, size);
                size -= Mathf.Max(0, size - size * bias);
            }
        }
    }
}