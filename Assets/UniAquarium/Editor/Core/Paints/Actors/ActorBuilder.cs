using System;
using System.Collections.Generic;

namespace UniAquarium.Core.Paints
{
    public readonly struct ActorBuilder
    {
        private readonly List<INode> _nodes;

        internal ActorBuilder(List<INode> nodes)
        {
            _nodes = nodes;
        }

        public void AddNode(INode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            _nodes.Add(node);
        }
    }
}
