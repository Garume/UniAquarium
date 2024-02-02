using System.Collections.Generic;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;

namespace UniAquarium.Aquarium.Actors
{
    internal sealed class Shockwave : AquariumActor
    {
        private readonly float _growingSpeed;
        private readonly MarbleCircle _shape;

        public Shockwave(AquariumSceneOption sceneOption, float growingSpeed = 1) : base(sceneOption)
        {
            _growingSpeed = growingSpeed;
            var colors = new Color[6];
            for (var i = 0; i < colors.Length; i++)
            {
                Color.RGBToHSV(Random.ColorHSV(), out var h, out var s, out var v);
                var color = Color.HSVToRGB(h,
                    s - 0.3f + Random.Range(0f, 1f) * 0.3f,
                    v - 0.5f + Random.Range(0f, 1f) * 0.5f);
                colors[i] = color;
            }

            _shape = new MarbleCircle(colors);
        }

        protected override IEnumerable<INode> CreateNodes()
        {
            return new INode[]
            {
                new RenderNode<AquariumSceneOption>(_shape)
            };
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _shape.Opacity -= deltaTime * 0.5f;
            Scale += _growingSpeed * deltaTime;

            if (_shape.Opacity <= 0) Destroy();
        }
    }
}