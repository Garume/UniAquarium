using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class SwayFallingNode : Node<AquariumSceneOption>
    {
        private readonly float _speed;
        private readonly float _waveWidth;
        private float _waveOffset;

        public SwayFallingNode(float speed = 0.5f, float waveWidth = 1f)
        {
            _speed = speed;
            _waveWidth = waveWidth;
        }

        public override void Update(float deltaTime)
        {
            _waveOffset += 0.01f;

            var toX = Mathf.Sin(_waveOffset) * _waveWidth * deltaTime + Transform.Position.x;
            var toY = _speed * deltaTime + Transform.Position.y;

            Transform.Position = new Vector2(toX, toY);
        }
    }
}