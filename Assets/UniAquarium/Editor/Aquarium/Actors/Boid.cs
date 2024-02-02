using System.Collections.Generic;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class Boid : AquariumActor
    {
        private BoidNode _boidNode;

        public Boid(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        protected override IEnumerable<INode> CreateNodes()
        {
            _boidNode = new BoidNode(0.5f, 10f, 0.2f);
            var trackingNode = new TargetTrackingNode(0.3f);
            return new INode[]
            {
                _boidNode,
                trackingNode
            };
        }

        public void AddTrackingNode(TargetTrackingNode targetTrackingNode)
        {
            _boidNode.AddTrackingNode(targetTrackingNode);
        }
    }
}