using System.Collections.Specialized;

namespace SonicNextModManager.Collections
{
    public class StackList<T> : List<T>, INotifyCollectionChanged
    {
        private int _capacity;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public StackList(int in_capacity)
        {
            _capacity = in_capacity;
        }

        public new void Add(T in_item)
        {
            if (Count >= _capacity)
                RemoveAt(Count - 1);

            Insert(0, in_item);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, in_item));
        }

        public T GetItemAt(int in_index)
        {
            if (Count == 0)
                return default;

            if (in_index >= Count)
                return this[Count - 1];

            return this[in_index];
        }

        public void Push(T in_item)
        {
            Add(in_item);
        }

        public T Pop()
        {
            T item = GetItemAt(0);

            RemoveAt(0);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

            return item;
        }

        public T Peek()
        {
            return GetItemAt(0);
        }
    }
}