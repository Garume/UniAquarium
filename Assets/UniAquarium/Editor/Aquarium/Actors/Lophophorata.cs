using System.Collections.Generic;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class Lophophorata : AquariumActor
    {
        private readonly Color _color;

        public Lophophorata(Color color, AquariumSceneOption sceneOption) : base(sceneOption)
        {
            _color = color;
        }

        protected override IEnumerable<INode> CreateNodes()
        {
            return new INode[]
            {
                new RenderNode<AquariumSceneOption>(new LophophorataShape(_color, 20))
            };
        }
    }
}