using System.Linq;
using UniAquarium.Core.Paints;
using UniAquarium.Foundation;
using UnityEngine;
using UnityEngine.UIElements;
using ITransform = UniAquarium.Core.Paints.ITransform;
using Random = UnityEngine.Random;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class BranchShape : Shape
    {
        private readonly float _angle;
        private readonly Color _color;
        private readonly int _maxSegmentPointSize;
        private readonly float[] _pointFlickingSpeeds;

        private readonly float[] _points;
        private readonly int _segmentLength;
        private readonly int _segments;

        private readonly float _speed;
        private float _wave;
        private float _wave2;

        public BranchShape(Color color, float angle, float speed)
        {
            _color = color;
            _angle = angle;
            _speed = speed;

            _wave = 0;
            _wave2 = Random.Range(0f, 1000f);
            _segments = Random.Range(3, 10);
            _segmentLength = Random.Range(50, 70);
            _maxSegmentPointSize = Random.Range(5, 10);

            _points = new float[_segments].Select(x => Random.Range(0f, _segments) + Random.Range(0f, 1f)).ToArray();
            _pointFlickingSpeeds = new float[_segments].Select(x => Random.Range(5f, 10f)).ToArray();
        }


        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            var bgc = new Color(_color.r, _color.g, _color.b, 0.12f);
            var pc = new Color(_color.r, _color.g, _color.b, 0.48f);
            var pc2 = new Color(_color.r, _color.g, _color.b, 0.09f);

            using (var painterScope = new Painter2DScope(painter))
            {
                painterScope.Translate(transform.Position);
                painterScope.Rotate(_angle * Mathf.Deg2Rad);

                for (var i = 0; i < _segments; i++)
                {
                    var theta = Mathf.Sin(_wave2) * 0.1f + Mathf.Sin(_wave) * 2f + Rand();
                    var to = Mathf.Lerp(_segmentLength, _segmentLength * 0.5f, (float)i / _segments);

                    _points[i] += _pointFlickingSpeeds[i] * deltaTime;
                    var width = Mathf.Abs(Mathf.Sin(_points[i])) * _maxSegmentPointSize;

                    painterScope.Rotate(theta * Mathf.Deg2Rad);
                    painterScope.DrawLine(Vector2.zero, new Vector2(0, to), width, bgc);
                    painterScope.FillCircle(Vector2.zero, width * 0.6f, pc);
                    painterScope.FillCircle(Vector2.zero, width * 2f, pc2);
                    painterScope.Translate(new Vector2(0, to));

                    _wave += _speed * 0.02f;
                    _wave2 += _speed * 0.002f * deltaTime;
                }

                var radius = Mathf.Abs(Mathf.Sin(_points[0])) * _maxSegmentPointSize;
                painterScope.FillCircle(Vector2.zero, radius * 0.3f, pc);
                painterScope.FillCircle(Vector2.zero, radius * 1f, pc2);
                _wave += _speed * deltaTime * 0.03f;
            }

            return;

            float Rand()
            {
                return Random.Range(0f, 1f) * 0.001f * (Random.Range(0, 1) == 0 ? -1 : 1);
            }
        }
    }
}