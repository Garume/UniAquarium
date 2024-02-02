using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal sealed class RenderNode<TOption> : Node<TOption> where TOption : ISceneOption
    {
        private readonly Shape _shape;

        public RenderNode(Shape shape)
        {
            _shape = shape;
        }

        public override void Draw(Painter2D painter, float deltaTime)
        {
            _shape.Draw(painter, Transform, deltaTime);
        }

        public override void Update(float deltaTime)
        {
        }
    }
}