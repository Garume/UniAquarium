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
        private const int HeadFrameStartAngle = 90;
        private const int HeadFrameEndAngle = 270;
        private readonly float _capPointAngleOffsetSpeed;

        private readonly float[] _capPointAngles;
        private readonly float[] _capPointCos;
        private readonly float[] _capPointPower;
        private readonly float[] _capPointSin;
        private readonly Color _color;
        private readonly Color _headFillColor;
        private readonly float _headSize;
        private readonly float _headWitherPower;
        private readonly Vector2[] _headFramePoints;
        private readonly Vector2[] _leftHeadPoints;
        private readonly Vector2[] _rightHeadPoints;

        private float _capPointAngleOffset;

        public JellyFishShape(Color color)
        {
            _color = new Color(color.r, color.g, color.b, 0.6f);
            _headFillColor = new Color(color.r, color.g, color.b, 0.6f);

            _capPointAngles = new float[CapJointCount];
            _capPointCos = new float[CapJointCount];
            _capPointPower = new float[CapJointCount];
            _capPointSin = new float[CapJointCount];
            _headFramePoints = new Vector2[CapJointCount];
            _leftHeadPoints = new Vector2[CapJointCount];
            _rightHeadPoints = new Vector2[CapJointCount];
            _capPointAngleOffset = 0f;
            _capPointAngleOffsetSpeed = 0.05f + Random.Range(0f, 1f) * 0.1f;
            _headWitherPower = 0.11f;
            _headSize = 10f;
        }

        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            AdvanceCapPointAngles(deltaTime);
            CalculateHeadEdgePoints(transform.Scale, _leftHeadPoints, _rightHeadPoints);

            using var painterScope = new Painter2DScope(painter);
            painterScope.Translate(transform.Position);
            painterScope.Rotate(transform.Rotation + 90f * Mathf.Deg2Rad);

            DrawFillHead(painterScope);
            DrawHeadFrame(painterScope, transform.Scale);
        }

        internal void AdvanceCapPointAngles(float deltaTime)
        {
            for (var i = 0; i < _capPointAngles.Length - 1; i++)
                _capPointAngles[i] = _capPointAngles[i + 1] + i;

            _capPointAngleOffset += _capPointAngleOffsetSpeed * deltaTime;
            _capPointAngles[^1] = Mathf.Abs(Mathf.Sin(_capPointAngleOffset)) * 30f + 20f;
        }

        internal void CalculateHeadEdgePoints(float scale, Vector2[] leftHeadPoints, Vector2[] rightHeadPoints)
        {
            UpdateCapPointCache();
            CalculateHeadPoints(scale, 1f, leftHeadPoints);
            CalculateHeadPoints(scale, -1f, rightHeadPoints);
        }

        private void UpdateCapPointCache()
        {
            for (var i = 0; i < _capPointAngles.Length; i++)
            {
                var angle = _capPointAngles[i] * Mathf.Deg2Rad;
                _capPointSin[i] = Mathf.Sin(angle);
                _capPointCos[i] = Mathf.Cos(angle);
                _capPointPower[i] = 1f - _headWitherPower * (i + 1);
            }
        }

        private void DrawHeadFrame(Painter2DScope painter, float scale)
        {
            painter.FillColor = _color;

            for (var r = HeadFrameStartAngle; r < HeadFrameEndAngle; r += (int)HeadDetail)
            {
                painter.BeginPath();
                AddHeadCurve(painter, scale, Mathf.Sin(r * Mathf.Deg2Rad), true);
                AddHeadCurve(painter, scale, Mathf.Sin((r + HeadDetail) * Mathf.Deg2Rad), false);
                painter.ClosePath();
                painter.Fill();
            }
        }

        private void DrawFillHead(Painter2DScope painter)
        {
            painter.FillColor = _headFillColor;

            painter.BeginPath();
            painter.MoveTo(Vector2.zero);

            for (var i = 0; i < _leftHeadPoints.Length; i++)
                painter.LineTo(_leftHeadPoints[i]);

            for (var i = _rightHeadPoints.Length - 1; i >= 0; i--)
                painter.LineTo(_rightHeadPoints[i]);

            painter.ClosePath();
            painter.Fill();
        }

        private void AddHeadCurve(Painter2DScope painter, float scale, float radialScale, bool forward)
        {
            CalculateHeadPoints(scale, radialScale, _headFramePoints);
            if (forward)
            {
                painter.MoveTo(_headFramePoints[0]);
                for (var i = 1; i < _headFramePoints.Length; i++)
                    painter.LineTo(_headFramePoints[i]);

                return;
            }

            for (var i = _headFramePoints.Length - 1; i >= 0; i--)
                painter.LineTo(_headFramePoints[i]);
        }

        private void CalculateHeadPoints(float scale, float radialScale, Vector2[] points)
        {
            var to = Vector2.zero;

            for (var i = 0; i < _capPointAngles.Length; i++)
            {
                to += CalculateHeadStep(i, scale, radialScale);
                points[i] = to;
            }
        }

        private Vector2 CalculateHeadStep(int index, float scale, float radialScale)
        {
            var size = _headSize * scale;
            return new Vector2(
                _capPointSin[index] * size * radialScale * _capPointPower[index],
                _capPointCos[index] * size
            );
        }
    }
}
