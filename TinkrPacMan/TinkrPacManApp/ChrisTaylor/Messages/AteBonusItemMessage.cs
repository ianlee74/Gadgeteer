// Copyright (c) 2012 Chris Taylor

using System;

namespace Pacman.Messages
{
  class AteBonusItemMessage
  {
    private static readonly AteBonusItemMessage _instance = new AteBonusItemMessage();

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Value { get; private set; }

    public static AteBonusItemMessage Message(int x, int y, int value)
    {
      _instance.X = x;
      _instance.Y = y;
      _instance.Value = value;
      return _instance;
    }

    private AteBonusItemMessage() { }
  }
}
