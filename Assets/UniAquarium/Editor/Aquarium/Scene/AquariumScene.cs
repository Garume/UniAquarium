using System.Collections.Generic;
using UniAquarium.Aquarium.Actors;
using UniAquarium.Core.Paints;

namespace UniAquarium.Aquarium.Scene
{
    public class AquariumScene : CanvasScene<AquariumActor>
    {
        private readonly List<Food> _foods = new();
        private readonly List<Shockwave> _shockwaves = new();

        internal List<Food> Foods => _foods;
        internal List<Shockwave> Shockwaves => _shockwaves;

        protected override void OnActorSpawned(AquariumActor actor)
        {
            switch (actor)
            {
                case Food food:
                    _foods.Add(food);
                    break;
                case Shockwave shockwave:
                    _shockwaves.Add(shockwave);
                    break;
            }
        }

        protected override void OnActorRemoved(AquariumActor actor)
        {
            switch (actor)
            {
                case Food food:
                    RemoveFood(food);
                    break;
                case Shockwave shockwave:
                    RemoveShockwave(shockwave);
                    break;
            }
        }

        private void RemoveFood(Food food)
        {
            for (var i = 0; i < _foods.Count; i++)
            {
                if (!ReferenceEquals(_foods[i], food)) continue;

                RemoveAtSwapBack(_foods, i);
                return;
            }
        }

        private void RemoveShockwave(Shockwave shockwave)
        {
            for (var i = 0; i < _shockwaves.Count; i++)
            {
                if (!ReferenceEquals(_shockwaves[i], shockwave)) continue;

                RemoveAtSwapBack(_shockwaves, i);
                return;
            }
        }

        private static void RemoveAtSwapBack<T>(List<T> list, int index)
        {
            var lastIndex = list.Count - 1;
            if (index != lastIndex)
                list[index] = list[lastIndex];

            list.RemoveAt(lastIndex);
        }
    }
}
