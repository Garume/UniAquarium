using UniAquarium.Aquarium.Actors;

namespace UniAquarium.Core.Paints
{
    public interface ISceneOption
    {
        float Width { get; }
        float Height { get; }
        ISceneUtility<AquariumActor> Utility { get; }
    }
}