using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Foundation
{
    internal sealed class Painter2DScope : IDisposable
    {
        private readonly Color _originalFillColor;
        private readonly LineCap _originalLineCap;
        private readonly LineJoin _originalLineJoin;
        private readonly float _originalLineWidth;
        private readonly float _originalMiterLimit;
        private readonly Color _originalStrokeColor;
        private readonly Gradient _originalStrokeGradient;
        private readonly Painter2D _painter2D;
        private Vector2 _position;
        private float _rotation;

        public Painter2DScope(Painter2D painter2D)
        {
            _painter2D = painter2D;
            _originalFillColor = painter2D.fillColor;
            _originalLineCap = painter2D.lineCap;
            _originalLineJoin = painter2D.lineJoin;
            _originalLineWidth = painter2D.lineWidth;
            _originalMiterLimit = painter2D.miterLimit;
            _originalStrokeColor = painter2D.strokeColor;
            _originalStrokeGradient = painter2D.strokeGradient;
        }

        public Color FillColor
        {
            set => _painter2D.fillColor = value;
        }

        public Color StrokeColor
        {
            set => _painter2D.strokeColor = value;
        }

        public Gradient StrokeGradient
        {
            set => _painter2D.strokeGradient = value;
        }

        public float LineWidth
        {
            set => _painter2D.lineWidth = value;
        }

        public LineCap LineCap
        {
            set => _painter2D.lineCap = value;
        }

        public LineJoin LineJoin
        {
            set => _painter2D.lineJoin = value;
        }

        public float MiterLimit
        {
            set => _painter2D.miterLimit = value;
        }

        public void Dispose()
        {
            if (_painter2D == null) return;

            _painter2D.fillColor = _originalFillColor;
            _painter2D.lineCap = _originalLineCap;
            _painter2D.lineJoin = _originalLineJoin;
            _painter2D.lineWidth = _originalLineWidth;
            _painter2D.miterLimit = _originalMiterLimit;
            _painter2D.strokeColor = _originalStrokeColor;
            _painter2D.strokeGradient = _originalStrokeGradient;
        }

        public void Rotate(float value)
        {
            _rotation += value;
        }

        public void Translate(Vector2 value)
        {
            _position = Transform(value);
        }

        public void BeginPath()
        {
            _painter2D?.BeginPath();
        }

        public void MoveTo(Vector2 position)
        {
            _painter2D?.MoveTo(Transform(position));
        }

        public void LineTo(Vector2 position)
        {
            _painter2D?.LineTo(Transform(position));
        }

        public void ClosePath()
        {
            _painter2D?.ClosePath();
        }

        public void Fill()
        {
            _painter2D?.Fill();
        }

        public void DrawLine(Vector2 from, Vector2 to, float width, Color color)
        {
            var transformedFrom = Transform(from);
            var transformedTo = Transform(to);
            _painter2D?.DrawLine(transformedFrom.x, transformedFrom.y, transformedTo.x, transformedTo.y, width, color);
        }

        public void DrawCircle(Vector2 position, float radius, float width, Color color)
        {
            var transformedPosition = Transform(position);

            _painter2D?.DrawCircle(transformedPosition.x, transformedPosition.y, radius, width, color);
        }

        public void FillCircle(Vector2 position, float radius, Color color)
        {
            var transformedPosition = Transform(position);
            _painter2D?.FillCircle(transformedPosition.x, transformedPosition.y, radius, color);
        }


        private Vector2 Transform(Vector2 position)
        {
            var cos = Mathf.Cos(_rotation);
            var sin = Mathf.Sin(_rotation);

            var rotatedX = cos * position.x - sin * position.y;
            var rotatedY = sin * position.x + cos * position.y;

            return new Vector2(rotatedX, rotatedY) + _position;
        }
    }
}