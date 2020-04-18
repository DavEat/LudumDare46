using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> {

    private T[] items;
    private int crtItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = crtItemCount;
        items[crtItemCount] = item;
        SortUp(item);
        crtItemCount++;
    }

    public T RemoveFirst()
    {
        T fisrtItem = items[0];
        crtItemCount--;
        items[0] = items[crtItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return fisrtItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count { get { return crtItemCount; } }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childItemLeft = item.HeapIndex * 2 + 1;
            int childItemRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childItemLeft < crtItemCount)
            {
                swapIndex = childItemLeft;
                if (childItemRight < crtItemCount)
                    if (items[childItemLeft].CompareTo(items[childItemRight]) < 0)
                        swapIndex = childItemRight;

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else return;
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }


}

public interface IHeapItem<T> :IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
