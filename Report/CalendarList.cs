using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Itenso.TimePeriod;
using iText.Commons.Bouncycastle.Asn1.X509;
using Truncon.Collections;

namespace AcaReader.Coverage;

public static class MonthsExtensions
{
    private static YearMonth[]? _months;
    public static IReadOnlyCollection<YearMonth> Months => _months ??= Enum.GetValues< YearMonth>().OrderBy(v => (int)v).ToArray();
}

public class CalendarList
{
    public IReadOnlyCollection<YearMonth> Months => MonthsExtensions.Months;
}

public class CalendarList<T> : CalendarList, IDictionary<YearMonth,T>
{
    private readonly T?[] _items = new T[MonthsExtensions.Months.Count()];
    private int Ix(YearMonth month) => (int)month - 1;
    public void Set(YearMonth month, T value) => _items[Ix(month)] = value;
    public T? Get(YearMonth month) => _items[Ix(month)];

    public bool Remove(YearMonth key)
    {
        if(Get(key) == null)
            return false;
        _items[Ix(key)] = default;
        return true;
    }

    public CalendarList() {}

    public IEnumerator<KeyValuePair<YearMonth, T>> GetEnumerator()
    {
        foreach(var m in Months)
            if(Get(m) is {} v)
                yield return new KeyValuePair<YearMonth, T>(m, v);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    void ICollection<KeyValuePair<YearMonth, T>>.Add(KeyValuePair<YearMonth, T> item) => Set(item.Key, item.Value);
    public void ForEach(Action<YearMonth> action)
    {
        foreach(var m in Months)
            action(m);
    }

    public void SetAll(Func<YearMonth,T> func)
    {
        ForEach(m => Set(m, func(m)));
    }
    public void Clear()
    {
        foreach (var kvp in this)
            Remove(kvp.Key);
    }

    public bool Contains(KeyValuePair<YearMonth, T> item) => Get(item.Key)?.Equals(item.Value) ?? item.Value == null;

    public void CopyTo(KeyValuePair<YearMonth, T>[] array, int arrayIndex) => 
        this.ToArray().CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<YearMonth, T> item)
    {
        if (Get(item.Key) == null)
            return false;

        Remove(item.Key);
        return true;
    }

    public int Count => Months.Count;

    bool ICollection<KeyValuePair<YearMonth, T>>.IsReadOnly => false;

    public void Add(YearMonth key, T value) => Set(key, value);

    public bool ContainsKey(YearMonth key) => Get(key) != null;

    public bool TryGetValue(YearMonth key, [MaybeNullWhen(false)]out T value)
    {
        value = Get(key);
        return value != null;
    }

    public T this[YearMonth key]
    {
        get => Get(key) ?? throw new KeyNotFoundException();
        set => Set(key,value);
    }

    private ICollection<YearMonth>? keys;
    public ICollection<YearMonth> Keys => keys ??= new KeyCollection(this);
    private ICollection<T>? values;
    public ICollection<T> Values => values ??= new ValueCollection(this);


    public sealed class KeyCollection : ICollection<YearMonth>, ICollection, IReadOnlyCollection<YearMonth>
    {
        private readonly CalendarList<T> _collection;

        internal KeyCollection(CalendarList<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _collection = collection;
        }

        void ICollection<YearMonth>.Add(YearMonth item)
        {
            throw new NotSupportedException();
        }

        void ICollection<YearMonth>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(YearMonth item)
        {
            return _collection.ContainsKey(item);
        }

        public void CopyTo(YearMonth[] array, int arrayIndex)
        {
            _collection.ToArray().CopyTo(array, arrayIndex);
        }

        public int Count => _collection.Count();

        bool ICollection<YearMonth>.IsReadOnly => true;

        bool ICollection<YearMonth>.Remove(YearMonth item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<YearMonth> GetEnumerator() => _collection.Select(kvp => kvp.Key).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collection).GetEnumerator();

        void ICollection.CopyTo(Array array, int index)
        {
            Array.Copy(_collection.ToArray(),array,index);
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => (_collection is ICollection coll) ? coll.SyncRoot : this;
    }
    
    public sealed class ValueCollection : ICollection<T>, ICollection, IReadOnlyCollection<T>
    {
        private readonly CalendarList<T> _collection;

        internal ValueCollection(CalendarList<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _collection = collection;
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            foreach(var kvp in _collection)
                if (kvp.Value?.Equals(item) == true || item == null)
                    return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _collection.ToArray().CopyTo(array, arrayIndex);
        }

        public int Count => _collection.Count();

        bool ICollection<T>.IsReadOnly => true;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator() => _collection.Select(kvp => kvp.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collection).GetEnumerator();

        void ICollection.CopyTo(Array array, int index)
        {
            Array.Copy(_collection.ToArray(),array,index);
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => (_collection is ICollection coll) ? coll.SyncRoot : this;
    }
}