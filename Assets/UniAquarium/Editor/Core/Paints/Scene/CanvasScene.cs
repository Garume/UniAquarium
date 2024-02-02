using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    public abstract class CanvasScene<TActor> : IPaintable, ISceneUtility<TActor> where TActor : IActor
    {
        private readonly List<IPaintable> _paintables = new();

        public void Draw(Painter2D painter, float deltaTime)
        {
            foreach (var paintable in _paintables) paintable.Draw(painter, deltaTime);
        }

        public void Update(float deltaTime)
        {
            for (var index = 0; index < _paintables.Count; index++)
            {
                var paintable = _paintables[index];
                paintable.Update(deltaTime);

                if (paintable is IDestroyable { IsDestroyed: true }) _paintables.RemoveAt(index);
            }
        }

        public void Instantiate<T>(T actor, Vector2? location, float angle, float scale) where T : IActor
        {
            actor.Position = location ?? Vector2.zero;
            actor.Rotation = angle;
            actor.Scale = scale;
            actor.Initialize();

            _paintables.Add(actor);
        }

        TActor ISceneUtility<TActor>.GetActor<T>()
        {
            return (T)_paintables.Find(actor => actor is T);
        }

        public void Press(MouseDownEvent evt)
        {
            for (var index = 0; index < _paintables.Count; index++)
            {
                var actor = _paintables[index];
                if (actor is IPressable pressable)
                    pressable.Press(evt);
            }
        }
    }
}