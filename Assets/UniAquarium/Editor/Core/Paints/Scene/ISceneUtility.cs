using UnityEngine;

namespace UniAquarium.Core.Paints
{
    public interface ISceneUtility<TActor> where TActor : IActor
    {
        TActor GetActor<T>() where T : TActor;
        void Instantiate<T>(T actor, Vector2? location, float angle, float scale) where T : IActor;
    }
}