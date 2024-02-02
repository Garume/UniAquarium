using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal abstract class SpawnerNode<T, TOption> : Node<TOption> where T : IActor where TOption : ISceneOption
    {
        public List<T> Actors { get; } = new();

        public override void Update(float deltaTime)
        {
            for (var index = 0; index < Actors.Count; index++)
            {
                var actor = Actors[index];
                if (actor.IsDestroyed)
                    Actors.Remove(actor);
            }
        }

        public override void Draw(Painter2D painter, float deltaTime)
        {
        }

        protected void Spawn(Vector2? location, float angle = 0, float scale = 1)
        {
            var actor = CreateActor();
            actor.Instantiate(location, angle, scale);
            Actors.Add(actor);
        }

        protected abstract T CreateActor();
    }
}