using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine.UIElements;

namespace UniAquarium.Aquarium.Nodes
{
    internal class ShockwaveSpawnerNode : SpawnerNode<Shockwave, AquariumSceneOption>, IPressable
    {
        public void Press(MouseDownEvent evt)
        {
            Spawn(evt.mousePosition);
        }

        protected override Shockwave CreateActor()
        {
            return new Shockwave(SceneOption, 30f);
        }
    }
}