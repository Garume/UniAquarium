using System.Collections.Generic;
using UniAquarium.Core.Paints;
using UniAquarium.Foundation;
using UnityEngine;
using UnityEngine.UIElements;
using ITransform = UniAquarium.Core.Paints.ITransform;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class JellyFishShape : Shape
    {
        private const int CapJointCount = 10;
        private const float HeadDetail = 30f;
        private readonly float _capPointAngleOffsetSpeed;

        private readonly float[] _capPointAngles;
        private readonly Color _color;
        private readonly Color _headFillColor;
        private readonly float _headSize;
        private readonly float _headWitherPower;

        private float _capPointAngleOffset;

        public JellyFishShape(Color color)
        {
            _color = new Color(color.r, color.g, color.b, 0.6f);
            _headFillColor = new Color(color.r, color.g, color.b, 0.6f);

            _capPointAngles = new float[CapJointCount];
            _capPointAngleOffset = 0f;
            _capPointAngleOffsetSpeed = 0.05f + Random.Range(0f, 1f) * 0.1f;
            _headWitherPower = 0.11f;
            _headSize = 10f;
        }


        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            var originPosition = transform.Position;
            var originRotation = transform.Rotation + 90f * Mathf.Deg2Rad;

            for (var i = 0; i < _capPointAngles.Length - 1; i++) _capPointAngles[i] = _capPointAngles[i + 1] + i;

            _capPointAngleOffset += _capPointAngleOffsetSpeed * deltaTime;
            _capPointAngles[^1] = Mathf.Abs(Mathf.Sin(_capPointAngleOffset)) * 30f + 20f;

            using var painterScope = new Painter2DScope(painter);
            painterScope.Translate(originPosition);
            painterScope.Rotate(originRotation);

            DrawFillHead(painterScope, transform.Scale);
            DrawHeadFrame(painterScope, transform.Scale);
        }

        private void DrawHeadFrame(Painter2DScope painter, float scale)
        {
            painter.FillColor = _color;
            painter.BeginPath();

            for (var r = 90; r <= 270f; r += 30)
            {
                var from = Vector2.zero;
                var to = Vector2.zero;
                var power = 1f;
                for (var i = 0; i < _capPointAngles.Length; i++)
                {
                    power -= _headWitherPower;
                    var angle = _capPointAngles[i];
                    from +=
                        new Vector2(
                            Mathf.Sin(angle * Mathf.Deg2Rad) * _headSize * scale *
                            Mathf.Sin(r * Mathf.Deg2Rad) * power,
                            Mathf.Cos(angle * Mathf.Deg2Rad) * _headSize * scale
                        );

                    to +=
                        new Vector2(
                            Mathf.Sin(angle * Mathf.Deg2Rad) * _headSize * scale *
                            Mathf.Sin((r + HeadDetail) * Mathf.Deg2Rad) * power,
                            Mathf.Cos(angle * Mathf.Deg2Rad) * _headSize * scale
                        );

                    if (r == 90 && i == 0)
                        painter.MoveTo(from);
                    else
                        painter.LineTo(from);

                    painter.LineTo(to);
                }
            }

            painter.ClosePath();
            painter.Fill();
        }

        private void DrawFillHead(Painter2DScope painter, float scale)
        {
            var power = 1f;
            var to = Vector2.zero;

            painter.FillColor = _headFillColor;

            painter.BeginPath();
            painter.MoveTo(Vector2.zero);

            var r = 90f;
            foreach (var angle in _capPointAngles)
            {
                power -= _headWitherPower;
                to +=
                    new Vector2(
                        Mathf.Sin(angle * Mathf.Deg2Rad) * _headSize * scale *
                        Mathf.Sin(r * Mathf.Deg2Rad) * power,
                        Mathf.Cos(angle * Mathf.Deg2Rad) * _headSize * scale
                    );

                painter.LineTo(to);
            }

            power = 1f;
            to = Vector2.zero;
            r = 270f;
            var stack = new Stack<Vector2>();

            foreach (var angle in _capPointAngles)
            {
                power -= _headWitherPower;
                to +=
                    new Vector2(
                        Mathf.Sin(angle * Mathf.Deg2Rad) * _headSize * scale *
                        Mathf.Sin(r * Mathf.Deg2Rad) * power,
                        Mathf.Cos(angle * Mathf.Deg2Rad) * _headSize * scale
                    );
                stack.Push(to);
            }

            while (stack.Count > 0) painter.LineTo(stack.Pop());

            painter.ClosePath();
            painter.Fill();
        }
    }
}