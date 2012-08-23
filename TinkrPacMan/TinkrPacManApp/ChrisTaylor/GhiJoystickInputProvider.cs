// Copyright (c) 2012 Chris Taylor

using System;
using dotnetwarrior.NetMF.Game.Input;
using Gadgeteer.Modules.GHIElectronics;

namespace Pacman
{
  class GhiJoystickInputProvider : IInputProvider
  {
    private Joystick _joystick;
    
    private bool _button1 = false;
    private Joystick.Position _position; 

    public GhiJoystickInputProvider(Joystick joystick)
    {
      if (joystick == null) throw new ArgumentNullException("joystick");
      _joystick = joystick;
      _joystick.JoystickPressed += joystick_JoystickPressed;
      _joystick.JoystickReleased += joystick_JoystickReleased;
    }

    void joystick_JoystickPressed(Joystick sender, Joystick.JoystickState state)
    {
      _button1 = true;
    }

    void joystick_JoystickReleased(Joystick sender, Joystick.JoystickState state)
    {
      _button1 = false;
    }

    public double X
    {
      get 
      {
        return (_position.X * 2) - 1;
      }
    }

    public double Y
    {
      get 
      {
        return (_position.Y * 2) - 1;
      }
    }

    public bool Button1
    {
      get { return _button1; }
    }

    public bool Button2
    {
      get { return false; }
    }

    public bool Button3
    {
      get { return false; }
    }

    public bool Button4
    {
      get { return false; }
    }

    public void Update(long elapsedTime)
    {
      _position = _joystick.GetJoystickPosition();
    }
  }
}
