namespace UniAquarium.Core.Paints
{
    public interface INode : IPaintable
    {
        void Initialize(ITransform transform, ISceneOption sceneOption);
    }
}
