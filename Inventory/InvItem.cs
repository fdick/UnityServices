using System;

[Serializable]
public class InvItem<T> where T : class, IItem
{
    public InvItem(T item, int count)
    {
        this.item = item;
        this.count = count;
        UniqueID = Guid.NewGuid();
    }

    public T item;
    public int count;
    public Guid UniqueID { get; private set; }
}