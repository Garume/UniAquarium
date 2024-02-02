namespace UniAquarium.Core.Paints
{
    public interface IActor : IPaintable, ITransform, IInstantiable<IActor>, IDestroyable
    {
        void Initialize();
        INode GetNode<T>() where T : INode;
    }
}