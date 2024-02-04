using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    public abstract class Actor<T> : IActor, IPressable where T : ISceneOption
    {
        private readonly List<INode> _nodes = new();

        public Actor(T sceneOption)
        {
            SceneOption = sceneOption;
            IsDestroyed = false;
        }

        protected T SceneOption { get; }

        public void Destroy()
        {
            IsDestroyed = true;
        }

        public bool IsDestroyed { get; private set; }


        public void Instantiate(Vector2? location, float angle = 0, float scale = 1)
        {
            SceneOption.Utility.Instantiate(this, location, angle, scale);
        }

        public virtual void Draw(Painter2D painter, float deltaTime)
        {
            foreach (var node in _nodes) node.Draw(painter, deltaTime);
        }

        public virtual void Update(float deltaTime)
        {
            foreach (var node in _nodes) node.Update(deltaTime);
        }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public virtual void Initialize()
        {
            foreach (var node in _nodes) node.Initialize(this, SceneOption);
        }

        public TNode GetNode<TNode>() where TNode : INode
        {
            foreach (var node in _nodes)
                if (node is TNode t)
                    return t;

            return default;
        }

        public void Press(MouseDownEvent evt)
        {
            foreach (var node in _nodes)
                if (node is IPressable pressable)
                    pressable.Press(evt);
        }


        protected void AddNode(INode node)
        {
            _nodes.Add(node);
        }

        protected void AddNodes(IEnumerable<INode> nodes)
        {
            _nodes.AddRange(nodes);
        }

        protected void RemoveNode(INode node)
        {
            _nodes.Remove(node);
        }
    }
}