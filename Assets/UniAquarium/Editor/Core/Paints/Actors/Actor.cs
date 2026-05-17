using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Paints
{
    public abstract class Actor<TActor, TOption> : IActor, IPressable
        where TActor : IActor
        where TOption : ISceneOption<TActor>
    {
        private readonly List<INode> _nodes = new();

        protected Actor(TOption sceneOption)
        {
            SceneOption = sceneOption;
            IsDestroyed = false;
        }

        protected TOption SceneOption { get; }

        public bool IsDestroyed { get; private set; }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public void Destroy()
        {
            IsDestroyed = true;
        }

        public virtual void Draw(Painter2D painter, float deltaTime)
        {
            for (var i = 0; i < _nodes.Count; i++)
                _nodes[i].Draw(painter, deltaTime);
        }

        public virtual void Update(float deltaTime)
        {
            for (var i = 0; i < _nodes.Count; i++)
                _nodes[i].Update(deltaTime);
        }

        public virtual void Initialize()
        {
            for (var i = 0; i < _nodes.Count; i++)
                _nodes[i].Initialize(this, SceneOption);
        }

        public TNode GetNode<TNode>() where TNode : INode
        {
            for (var i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                if (node is TNode t)
                    return t;
            }

            return default;
        }

        public void Press(MouseDownEvent evt)
        {
            for (var i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                if (node is IPressable pressable)
                    pressable.Press(evt);
            }
        }

        protected void AddNode(INode node)
        {
            _nodes.Add(node);
        }

        protected void RemoveNode(INode node)
        {
            _nodes.Remove(node);
        }
    }
}
