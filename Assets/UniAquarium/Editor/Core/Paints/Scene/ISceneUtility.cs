using System.Collections.Generic;
using UnityEngine;

namespace UniAquarium.Core.Paints
{
    public interface ISceneUtility<TActor> where TActor : IActor
    {
        IReadOnlyList<TActor> Actors { get; }
        int GetActors<T>(List<T> results) where T : TActor;
        void Spawn<T>(T actor, Vector2? location, float angle, float scale) where T : TActor;
    }
}
