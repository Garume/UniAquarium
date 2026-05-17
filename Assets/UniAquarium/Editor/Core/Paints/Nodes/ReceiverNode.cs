using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal abstract class ReceiverNode<T, TOption> : Node<TOption> where TOption : ISceneOption
    {
        public T ReceivedItem { get; private set; }
        public bool HasReceivedItem { get; private set; }

        public override void Draw(Painter2D painter, float deltaTime)
        {
        }

        protected abstract bool TryUpdateReceived(out T receivedItem);

        public override void Update(float deltaTime)
        {
            HasReceivedItem = TryUpdateReceived(out var receivedItem);
            ReceivedItem = receivedItem;
        }
    }
}
