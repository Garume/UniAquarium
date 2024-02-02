using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal abstract class ReceiverNode<T, TOption> : Node<TOption> where TOption : ISceneOption
    {
        public T ReceivedItem { get; private set; }

        public override void Draw(Painter2D painter, float deltaTime)
        {
        }

        protected abstract T UpdateReceived();

        public override void Update(float deltaTime)
        {
            ReceivedItem = UpdateReceived();
        }
    }
}