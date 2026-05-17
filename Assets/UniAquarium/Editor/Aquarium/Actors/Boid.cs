using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class Boid : AquariumActor
    {
        private BoidNode _boidNode;

        public Boid(AquariumSceneOption sceneOption) : base(sceneOption)
        {
        }

        protected override void CreateNodes()
        {
            _boidNode = new BoidNode(0.5f, 10f, 0.2f);
            AddNode(_boidNode);
            AddNode(new TargetTrackingNode(0.3f));
        }

        public void AddTrackingNode(TargetTrackingNode targetTrackingNode)
        {
            _boidNode.AddTrackingNode(targetTrackingNode);
        }
    }
}
