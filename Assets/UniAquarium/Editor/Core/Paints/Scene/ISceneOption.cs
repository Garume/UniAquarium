namespace UniAquarium.Core.Paints
{
    public interface ISceneOption
    {
        float Width { get; }
        float Height { get; }
    }

    public interface ISceneOption<TActor> : ISceneOption where TActor : IActor
    {
        ISceneUtility<TActor> Utility { get; }
    }
}
