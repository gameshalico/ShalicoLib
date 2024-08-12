namespace ShalicoLib.Data
{
    public class Storage<T> : IReadOnlyStorage<T>
    {
        public bool HasContent { get; private set; }

        public T Content { get; private set; }

        public bool TryGet(out T value)
        {
            if (!HasContent)
            {
                value = default;
                return false;
            }

            value = Content;
            return true;
        }

        public void Set(T value)
        {
            Content = value;
            HasContent = true;
        }

        public void Clear()
        {
            Content = default;
            HasContent = false;
        }
    }
}