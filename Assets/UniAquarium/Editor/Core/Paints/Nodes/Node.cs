using System;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    internal class Node<TOption> : INode, IEquatable<INode> where TOption : ISceneOption
    {
        private readonly Guid _id = Guid.NewGuid();

        public TOption SceneOption { get; private set; }
        public ITransform Transform { get; private set; }

        public bool Equals(INode other)
        {
            return other != null && _id.Equals(other.Id);
        }
        
        public virtual void Draw(Painter2D painter, float deltaTime)
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        Guid INode.Id => _id;

        public void Initialize(ITransform transform, ISceneOption sceneOption)
        {
            Transform = transform;
            SceneOption = (TOption)sceneOption;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((INode)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}