// Copyright (c) 2012 Chris Taylor

using System;

namespace Pacman.Messages
{
  class CollidedWithPacmanMessage
  {
    private static readonly CollidedWithPacmanMessage _instance = new CollidedWithPacmanMessage();

    public static CollidedWithPacmanMessage Message(Ghost ghost)
    {
      _instance.Ghost = ghost;
      return _instance;
    }

    public Ghost Ghost { get; private set; }

    private CollidedWithPacmanMessage() { }  
  }
}
