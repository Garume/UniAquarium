namespace UniAquarium.Core.Paints
{
    public interface IDestroyable
    {
        bool IsDestroyed { get; }
        void Destroy();
    }
}