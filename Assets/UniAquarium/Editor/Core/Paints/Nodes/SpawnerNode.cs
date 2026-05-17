using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal abstract class SpawnerNode<T, TActor, TOption> : Node<TOption>
        where T : TActor
        where TActor : IActor
        where TOption : ISceneOption<TActor>
    {
        public override void Update(float deltaTime)
        {
        }

        public override void Draw(Painter2D painter, float deltaTime)
        {
        }

        protected void Spawn(Vector2? location, float angle = 0, float scale = 1)
        {
            var actor = CreateActor();
            SceneOption.Utility.Spawn(actor, location, angle, scale);
        }

        protected abstract T CreateActor();
    }
}
