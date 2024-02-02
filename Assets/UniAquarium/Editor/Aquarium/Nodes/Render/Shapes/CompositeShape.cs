using System.Collections;
using System.Collections.Generic;
using UniAquarium.Core.Paints;
using UnityEngine.UIElements;
using ITransform = UniAquarium.Core.Paints.ITransform;

namespace UniAquarium.Aquarium.Nodes
{
    internal class CompositeShape : Shape, ICollection<Shape>
    {
        private readonly List<Shape> _shapes = new();

        public int Count => _shapes.Count;

        public bool IsReadOnly => false;

        public void Add(Shape item)
        {
            _shapes.Add(item);
        }

        public void Clear()
        {
            _shapes.Clear();
        }

        public bool Contains(Shape item)
        {
            return _shapes.Contains(item);
        }

        public void CopyTo(Shape[] array, int arrayIndex)
        {
            _shapes.CopyTo(array, arrayIndex);
        }

        public bool Remove(Shape item)
        {
            return _shapes.Remove(item);
        }

        public IEnumerator<Shape> GetEnumerator()
        {
            return _shapes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override void Draw(Painter2D painter, ITransform transform, float deltaTime)
        {
            foreach (var shape in _shapes) shape.Draw(painter, transform, deltaTime);
        }
    }
}