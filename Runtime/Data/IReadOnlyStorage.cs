namespace ShalicoLib.Data
{
    public interface IReadOnlyStorage<T>
    {
        bool HasContent { get; }
        T Content { get; }
        bool TryGet(out T value);
    }
}