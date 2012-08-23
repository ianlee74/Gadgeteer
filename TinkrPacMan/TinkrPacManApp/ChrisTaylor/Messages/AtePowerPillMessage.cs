// Copyright (c) 2012 Chris Taylor

using System;

namespace Pacman.Messages
{
  class AtePowerPillMessage
  {
    private static readonly AtePowerPillMessage _instance = new AtePowerPillMessage();

    public static AtePowerPillMessage Message()
    {
      return _instance;
    }

    private AtePowerPillMessage() { }
  }
}
