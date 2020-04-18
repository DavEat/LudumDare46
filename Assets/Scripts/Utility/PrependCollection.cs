using System.Collections.Generic;

public class PrependList<T>
{
    Collection first = null;

    const int MAXLENGTH = 25;
    int m_length = 0;
    public int length { get { return m_length; } }

    public void Add(T t)
    {
        m_length++;
        first = new Collection(first, t);

        if (m_length >= MAXLENGTH)
        {
            RemoveLast();
        }
    }
    public T GetFirst()
    {
        return first != null ? first.obj : default(T);
    }
    public void RemoveFirst()
    {
        if (first != null)
        {
            m_length--;
            first = first.next;
        }
    }
    void RemoveLast()
    {
        Collection collection = first;
        List<Collection> collections = new List<Collection>();
        int count = 0;
        while (collection != null)
        {
            count++;
            collection = collection.next;
            if (count >= MAXLENGTH)
                collections.Add(collection);
        }
        foreach (Collection c in collections)
        {
            if (c != null)
                c.next = null;
        }
    }

    int Count
    {
        get
        {
            int count = 0;

            Collection collection = first;

            while (collection != null)
            {
                count++;
                collection = collection.next;
            }

            return count;
        }
    }

    public class Collection
    {
        public Collection next;
        public T obj;

        public Collection(Collection next, T obj)
        {
            this.next = next;
            this.obj = obj;
        }
    }
}