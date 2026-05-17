using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    public class Node<TOption> : INode where TOption : ISceneOption
    {
        public TOption SceneOption { get; private set; }
        public ITransform Transform { get; private set; }

        public virtual void Draw(Painter2D painter, float deltaTime)
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public void Initialize(ITransform transform, ISceneOption sceneOption)
        {
            Transform = transform;
            SceneOption = (TOption)sceneOption;
        }
    }
}
