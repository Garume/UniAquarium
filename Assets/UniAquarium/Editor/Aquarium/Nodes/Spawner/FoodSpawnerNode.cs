using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Scene;
using UniAquarium.Core.Paints;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniAquarium.Aquarium.Nodes
{
    internal sealed class FoodSpawnerNode : SpawnerNode<Food, AquariumSceneOption>, IPressable
    {
        public void Press(MouseDownEvent evt)
        {
            var location = new Vector2(evt.mousePosition.x, 0);
            Spawn(location);
        }

        protected override Food CreateActor()
        {
            return new Food(SceneOption);
        }
    }
}