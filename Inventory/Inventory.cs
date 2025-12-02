using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

/// <summary>
/// Simple inventory with countable opportunity
/// </summary>
/// <typeparam name="T">Type of contained items</typeparam>
/// <typeparam name="T1">Type of unique item names. This is enum! This need only for comfortable use</typeparam>
[Serializable]
public class Inventory<T> : IEnumerable<T> where T : class, IItem
{
    private InvItem<T>[] _itemsVault;

    private int _capacity;
    private int _countBusySlots;

    public int CountBusySlots => _countBusySlots;

    public InvItem<T>[] Vault => _itemsVault;
    public int Capacity => _capacity;

    public Inventory(int capacity)
    {
        _capacity = capacity;
        _itemsVault = new InvItem<T>[_capacity];
    }


    public IEnumerator<T> GetEnumerator()
    {
        return Vault.GetEnumerator() as IEnumerator<T>;
    }

    ~Inventory()
    {
        DestroyInventory();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int AddItem(T item, int count, out int inventoryIndex)
    {
        return AddItem(new InvItem<T>(item, count), out inventoryIndex);
    }

    /// <summary>
    /// Adds invItem to inventory. 
    /// </summary>
    /// <param name="invItem"></param>
    /// <returns>-1 means cant add this item. 0 means was added part of quantity item and has an unused rest. 1 means completely added item. </returns>
    public int AddItem(InvItem<T> invItem, out int outInventoryIndex)
    {
        outInventoryIndex = 0;
        if (invItem == null)
        {
            Debug.Log("Input object is null");
            return -1;
        }

        if (invItem.item.IsCountable)
        {
            return TryAddCountableItem(invItem, out outInventoryIndex);
        }

        for (int i = 0; i < invItem.count; i++)
        {
            if (!HaveIFreeSlots())
            {
                Debug.Log("Inventory is full");
                return -1;
            }

            outInventoryIndex = GetFreeSlotId();
            if (outInventoryIndex == -1)
            {
                Debug.Log("Inventory is full!");
                return -1;
            }

            _itemsVault[outInventoryIndex] = invItem;
            _countBusySlots++;
        }

        return 1;
    }

    private bool RemoveItem(InvItem<T> item)
    {
        if (item == null || !Contains(item, out int outItemId))
        {
            Debug.LogError("Inventory cant delete this item");
            return false;
        }

        _itemsVault[outItemId] = null;
        _countBusySlots--;
        return true;
    }

    /// <summary>
    /// Remove item from inventory.
    /// </summary>
    /// <param name="item">Item of vault</param>
    /// <param name="removableCount">If == 0 than full remove item</param>
    /// <returns> If 0 - was removed the full item. If 1 - was removed one of count. If -1 - was not removed the item</returns>
    public int RemoveItem(InvItem<T> item, int removableCount = 0)
    {
        if (item == null || !Contains(item, out int outItemId))
        {
            Debug.LogError("Inventory cant delete this item");
            return -1;
        }

        if (removableCount == 0 || !item.item.IsCountable)
            return RemoveItem(item) ? 0 : -1;
        else
        {
            item.count -= removableCount;
            if (item.count == 0)
                return RemoveItem(item) ? 0 : -1;
        }

        return 1;
    }

    private bool RemoveAt(int index)
    {
        if (index >= Capacity)
        {
            Debug.LogError("Inventory cant delete this item");
            return false;
        }

        _itemsVault[index] = default;
        _countBusySlots--;

        return true;
    }

    /// <summary>
    /// Remove item from inventory by index.
    /// </summary>
    /// <param name="index">Index of vault.</param>
    /// <param name="removableCount">If == 0 than full remove item.</param>
    public bool RemoveAt(int index, int removableCount = 0)
    {
        if (index >= Capacity)
        {
            Debug.LogError("Inventory cant delete this item");
            return false;
        }

        var item = _itemsVault[index];
        if (removableCount == 0 || !item.item.IsCountable)
            return RemoveAt(index);
        else
        {
            item.count -= removableCount;
            if (item.count == 0)
                return RemoveAt(index);
        }

        return true;
    }

    public bool HaveIFreeSlots()
    {
        return _countBusySlots < _capacity;
    }

    public bool Contains(InvItem<T> item, out int outItemIndex)
    {
        outItemIndex = -1;
        if (item == null)
            return false;
        for (int i = 0; i < _itemsVault.Length; i++)
        {
            if (_itemsVault[i] == null)
                continue;

            if (item.UniqueID == _itemsVault[i].UniqueID)
            {
                outItemIndex = i;
                return true;
            }
        }

        //if not contain than return false
        outItemIndex = -1;
        return false;
    }

    public bool Contains(Type type, out int outItemIndex)
    {
        outItemIndex = -1;
        for (int i = 0; i < _itemsVault.Length; i++)
        {
            var item = _itemsVault[i];

            if (item == null)
                continue;

            if (item.item.GetType() == type)
            {
                outItemIndex = i;
                return true;
            }
        }

        return false;
    }

    public int GetFreeSlotId()
    {
        for (int i = 0; i < _itemsVault.Length; i++)
        {
            if (_itemsVault[i] == null || _itemsVault[i].item == null)
                return i;
        }

        return -1;
    }

    public InvItem<T> this[int index]
    {
        get => _itemsVault[index];
        set
        {
            if (_itemsVault[index] == null && value != null)
                _countBusySlots++;
            else if (_itemsVault[index] != null && value == null)
                _countBusySlots--;

            _itemsVault[index] = value;
        }
    }

    public int IndexOf(InvItem<T> countableItem)
    {
        if (Contains(countableItem, out var index))
            return index;
        Debug.LogError($"Invalid index - {index}. Vault count - {Capacity}");
        return -1;
    }

    public void Sort(IComparer<InvItem<T>> comparer = null)
    {
        if (comparer == null)
        {
            _itemsVault = _itemsVault.OrderByDescending(x => x != null ? 1 : -1).ToArray();
            return;
        }

        _itemsVault = _itemsVault.OrderByDescending(x => x, comparer).ToArray();
    }

    public void ResizeInventory(int newCapacity)
    {
        if (newCapacity <= 0)
            return;

        var vault = new InvItem<T>[newCapacity];

        Sort(); // we need position all items in stroke
        Array.Copy(_itemsVault, vault, _countBusySlots > newCapacity ? _countBusySlots - newCapacity : _countBusySlots);

        //clear
        _itemsVault = null;
        // _countBusySlots = 0;

        //init new vault
        _itemsVault = vault;
        _capacity = newCapacity;
    }

    public void Clear()
    {
        _itemsVault = new InvItem<T>[_capacity];
        _countBusySlots = 0;
    }

    public bool IsEmpty()
    {
        return _countBusySlots == 0;
    }

    private void DestroyInventory()
    {
        _itemsVault = null;
    }

    private int TryAddCountableItem(InvItem<T> invItem, out int outItemIndex)
    {
        outItemIndex = -1;
        if (!invItem.item.IsCountable || invItem.count <= 0)
            return -1;
        var notFilledItem = GetNotFilledCountableItem(invItem.item.UniqueName, out outItemIndex);
        if (notFilledItem != null)
        {
            var endCount = notFilledItem.count + invItem.count;
            var rest = 0;
            if (endCount > notFilledItem.item.MaxCounts)
            {
                rest = endCount - notFilledItem.item.MaxCounts;
                endCount = notFilledItem.item.MaxCounts;
            }

            notFilledItem.count = endCount;
            invItem.count = rest;

            //if have rest(остаток)
            if (rest > 0)
                return TryAddCountableItem(invItem, out outItemIndex);
            return 1;
        }
        else
        {
            var endCount = invItem.count > invItem.item.MaxCounts ? invItem.item.MaxCounts : invItem.count;
            var rest = 0;
            if (invItem.count > invItem.item.MaxCounts)
                rest = invItem.count - invItem.item.MaxCounts;


            var freeIndex = GetFreeSlotId();
            if (freeIndex == -1)
            {
                Debug.Log($"Inventory is full!. Capacity {_capacity}. Filled slots {_countBusySlots}");
                return -1;
            }

            var newInvItem = new InvItem<T>(invItem.item, endCount);
            _itemsVault[freeIndex] = newInvItem;

            if (rest > 0)
            {
                invItem.count = rest;
                return TryAddCountableItem(invItem, out outItemIndex);
            }

            outItemIndex = freeIndex;
            _countBusySlots++;
            return 1;
        }
    }

    [CanBeNull]
    private InvItem<T> GetNotFilledCountableItem(string uniqueName, out int itemIndex)
    {
        itemIndex = -1;
        for (int i = 0; i < _itemsVault.Length; i++)
        {
            var it = _itemsVault[i];
            if (it == null || it.item.UniqueName != uniqueName || it.count == it.item.MaxCounts)
                continue;
            itemIndex = i;
            return it;
        }

        return null;
    }
}