// Copyright (c) 2012 Chris Taylor

using System;

namespace dotnetwarrior.NetMF.Game
{  
  internal class GameObjectContainer 
  {
    private const int DefaultSize = 4;

    private GameObject[] _items = new GameObject[DefaultSize];
    private int _count;

    public void Add(GameObject item)
    {
      if (_count + 1 == _items.Length)
      {
        EnsureCapacity(_count + 1);
      }
      _items[_count++] = item;
    }

    public int Count { get { return _count; } }

    public GameObject this[int index]
    {
      get { return _items[index]; }
    }

    private void EnsureCapacity(int required)
    {
      if (_items == null)
      {
        _items = new GameObject[DefaultSize];
      }
      else if (required >= _items.Length)
      {
        GameObject[] newItems = new GameObject[_items.Length * 2];
        Array.Copy(_items, newItems, _items.Length);
        _items = newItems;
      }      
    }
  }
}
