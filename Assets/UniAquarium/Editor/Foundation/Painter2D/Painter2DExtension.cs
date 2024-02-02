using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Foundation
{
    internal static class Painter2DExtension
    {
        public static void MoveTo(this Painter2D painter, float x, float y)
        {
            var moveTo = new Vector2(x, y);
            painter.MoveTo(moveTo);
        }

        public static void LineTo(this Painter2D painter, float x, float y)
        {
            var lineTo = new Vector2(x, y);
            painter.LineTo(lineTo);
        }

        public static void QuadraticCurveTo(this Painter2D painter, float x1, float y1, float x2, float y2)
        {
            var quadraticCurveTo1 = new Vector2(x1, y1);
            var quadraticCurveTo2 = new Vector2(x2, y2);
            painter.QuadraticCurveTo(quadraticCurveTo1, quadraticCurveTo2);
        }

        public static void BezierCurveTo(this Painter2D painter, float x1, float y1, float x2, float y2, float x3,
            float y3)
        {
            var bezierCurveTo1 = new Vector2(x1, y1);
            var bezierCurveTo2 = new Vector2(x2, y2);
            var bezierCurveTo3 = new Vector2(x3, y3);
            painter.BezierCurveTo(bezierCurveTo1, bezierCurveTo2, bezierCurveTo3);
        }

        public static void FillRect(this Painter2D painter, float x, float y, float width, float height)
        {
            painter.BeginPath();
            painter.MoveTo(x, y);
            painter.LineTo(x + width, y);
            painter.LineTo(x + width, y + height);
            painter.LineTo(x, y + height);
            painter.ClosePath();
            painter.Fill();
        }

        public static void DrawLine(this Painter2D painter, float x1, float y1, float x2, float y2)
        {
            painter.BeginPath();
            painter.MoveTo(x1, y1);
            painter.LineTo(x2, y2);
            painter.ClosePath();
            painter.Stroke();
        }

        public static void DrawLine(this Painter2D painter, float x1, float y1, float x2, float y2, float width,
            Color color)
        {
            painter.lineWidth = width;
            painter.strokeColor = color;
            painter.DrawLine(x1, y1, x2, y2);
        }

        public static void FillCircle(this Painter2D painter, float x, float y, float radius)
        {
            var center = new Vector2(x, y);

            painter.BeginPath();
            painter.Arc(center, radius, 0, Angle.Degrees(360));
            painter.ClosePath();
            painter.Fill();
        }

        public static void FillCircle(this Painter2D painter, float x, float y, float radius, Color color)
        {
            painter.fillColor = color;
            painter.FillCircle(x, y, radius);
        }


        public static void DrawCircle(this Painter2D painter, float x, float y, float radius)
        {
            var center = new Vector2(x, y);

            painter.BeginPath();
            painter.Arc(center, radius, 0, Angle.Degrees(360));
            painter.ClosePath();
            painter.Stroke();
        }

        public static void DrawCircle(this Painter2D painter, float x, float y, float radius, float width, Color color)
        {
            painter.lineWidth = width;
            painter.strokeColor = color;
            painter.DrawCircle(x, y, radius);
        }
    }
}