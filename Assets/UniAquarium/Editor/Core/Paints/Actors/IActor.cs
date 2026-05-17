namespace UniAquarium.Core.Paints
{
    public interface IActor : IPaintable, ITransform, IDestroyable
    {
        void Initialize();
        T GetNode<T>() where T : INode;
    }
}
