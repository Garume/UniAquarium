using UniAquarium.Core.Paints;
using UniAquarium.Foundation;
using UnityEngine;
using UnityEngine.UIElements;
using ITransform = UniAquarium.Core.Paints.ITransform;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class FishShape : Shape
    {
        private const float SizeBias = 0.3f;
        private const float SegLength = 14f;
        private readonly Color _color;


        private readonly Vector2[] _segments = new Vector2[10];

        private float _lastAngle;
        private float _lastDirection = 30;

        public FishShape(Color? color = null)
        {
            _color = color ?? Random.ColorHSV();

            for (var i = 0; i < _segments.Length; i++) _segments[i] = new Vector2(0, 0);
        }

        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            DrawSegment(painter, 0, transform.Position.x, transform.Position.y, transform.Scale, deltaTime);
            for (var i = 0; i < 8; i++)
                DrawSegment(painter, i + 1, _segments[i].x, _segments[i].y, transform.Scale, deltaTime);
        }

        private void DrawSegment(Painter2D painter, int number, float x, float y, float scale, float deltaTime)
        {
            var size = scale * SizeBias;
            var dx = x - _segments[number].x;
            var dy = y - _segments[number].y;
            var angle = Mathf.Atan2(dy, dx);

            var x2 = x - Mathf.Cos(angle) * SegLength * size;
            var y2 = y - Mathf.Sin(angle) * SegLength * size;
            _segments[number] = new Vector2(x2, y2);

            painter.fillColor = _color;
            painter.strokeColor = _color;

            if (number == 1)
            {
                _lastAngle = Mathf.Lerp(_lastAngle, _lastDirection, 0.4f) * deltaTime;
                _lastDirection = _lastAngle switch
                {
                    >= 13 => 0,
                    <= 2 => 15,
                    _ => _lastDirection
                };

                foreach (var sign in new[] { -1, 1 })
                {
                    painter.lineWidth = 3 * size;
                    painter.DrawLine(
                        x + Mathf.Cos(angle + 120f * sign * Mathf.Deg2Rad) * 10 * size,
                        y + Mathf.Sin(angle + 120f * sign * Mathf.Deg2Rad) * 10 * size,
                        x + Mathf.Cos(angle + (145f + _lastAngle) * sign * Mathf.Deg2Rad) * 45 * size,
                        y + Mathf.Sin(angle + (145f + _lastAngle) * sign * Mathf.Deg2Rad) * 45 * size
                    );

                    painter.FillCircle(
                        x + Mathf.Cos(angle + (145f + _lastAngle) * sign * Mathf.Deg2Rad) * 45 * size,
                        y + Mathf.Sin(angle + (145f + _lastAngle) * sign * Mathf.Deg2Rad) * 45 * size,
                        4 * size
                    );
                }

                painter.FillCircle(
                    _segments[number].x,
                    _segments[number].y,
                    (10 - number) * 1.2f * size
                );
            }

            else if (number % 2 == 1)
            {
                painter.FillCircle(
                    _segments[number].x,
                    _segments[number].y,
                    1.5f * size
                );

                painter.lineWidth = 1;
                painter.DrawCircle(
                    _segments[number].x,
                    _segments[number].y,
                    (10 - number) * 1.2f * size
                );
            }
            else
            {
                painter.FillCircle(
                    _segments[number].x,
                    _segments[number].y,
                    (10 - number) * 0.5f * size);
            }
        }
    }
}