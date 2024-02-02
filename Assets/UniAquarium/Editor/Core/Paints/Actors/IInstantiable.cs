using UnityEngine;

namespace UniAquarium.Core.Paints
{
    public interface IInstantiable<in T> where T : IPaintable
    {
        void Instantiate(Vector2? location, float angle = 0, float scale = 1);
    }
}