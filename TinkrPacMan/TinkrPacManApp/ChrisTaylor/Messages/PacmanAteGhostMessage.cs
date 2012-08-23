// Copyright (c) 2012 Chris Taylor

using System;

namespace Pacman.Messages
{
  class PacmanAteGhostMessage
  {
    private static readonly PacmanAteGhostMessage _instance = new PacmanAteGhostMessage();

    public int X { get; private set; }
    public int Y { get; private set; }    
    public int Value { get; private set; }

    public static PacmanAteGhostMessage Message(int x, int y, int value)
    {
      _instance.X = x;
      _instance.Y = y;
      _instance.Value = value;    
      return _instance;
    }

    private PacmanAteGhostMessage() { }
  }  
}
