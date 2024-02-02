using System;

namespace UniAquarium.Core.Paints
{
    public interface INode : IPaintable
    {
        Guid Id { get; }
        void Initialize(ITransform transform, ISceneOption sceneOption);
    }
}