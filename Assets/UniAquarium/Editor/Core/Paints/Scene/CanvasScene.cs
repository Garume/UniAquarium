using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    public abstract class CanvasScene<TActor> : IPaintable, ISceneUtility<TActor> where TActor : IActor
    {
        private readonly List<TActor> _actors = new();

        public IReadOnlyList<TActor> Actors => _actors;

        public void Draw(Painter2D painter, float deltaTime)
        {
            for (var i = 0; i < _actors.Count; i++)
                _actors[i].Draw(painter, deltaTime);
        }

        public void Update(float deltaTime)
        {
            for (var index = 0; index < _actors.Count; index++)
            {
                var actor = _actors[index];
                actor.Update(deltaTime);

                if (!actor.IsDestroyed) continue;

                RemoveAtSwapBack(index);
                index--;
            }
        }

        public void Spawn<T>(T actor, Vector2? location, float angle, float scale) where T : TActor
        {
            if (actor == null) throw new ArgumentNullException(nameof(actor));
            if (ContainsActor(actor))
                throw new InvalidOperationException("The actor has already been spawned in this scene.");

            actor.Position = location ?? Vector2.zero;
            actor.Rotation = angle;
            actor.Scale = scale;
            actor.Initialize();

            _actors.Add(actor);
            OnActorSpawned(actor);
        }

        public int GetActors<T>(List<T> results) where T : TActor
        {
            var count = 0;
            for (var i = 0; i < _actors.Count; i++)
            {
                if (_actors[i] is T actor)
                {
                    results.Add(actor);
                    count++;
                }
            }

            return count;
        }

        private bool ContainsActor(TActor actor)
        {
            for (var i = 0; i < _actors.Count; i++)
                if (ReferenceEquals(_actors[i], actor))
                    return true;

            return false;
        }

        public void Press(MouseDownEvent evt)
        {
            for (var index = 0; index < _actors.Count; index++)
            {
                var actor = _actors[index];
                if (actor is IPressable pressable)
                    pressable.Press(evt);
            }
        }

        protected virtual void OnActorSpawned(TActor actor)
        {
        }

        protected virtual void OnActorRemoved(TActor actor)
        {
        }

        private void RemoveAtSwapBack(int index)
        {
            var actor = _actors[index];
            OnActorRemoved(actor);

            var lastIndex = _actors.Count - 1;
            if (index != lastIndex)
                _actors[index] = _actors[lastIndex];

            _actors.RemoveAt(lastIndex);
        }
    }
}
