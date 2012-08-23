// Copyright (c) 2012 Chris Taylor

using System;

namespace dotnetwarrior.NetMF.Game.Input
{
  /// <summary>
  /// Manages the state of one or more input providers
  /// The data is collected from each registered input provider
  /// and a unified result is exposed which represents the state of all the providers.
  /// The X and Y locations represent the average X and Y value returned from all providers
  /// Button states are true if any one of the providers returns true for the button.
  /// </summary>
  public class InputManager
  {
    private const int MaxProviders = 4;
    private int _count = 0;
    private IInputProvider[] _providers = new IInputProvider[MaxProviders];

    /// <summary>
    /// X position of the input device.
    /// -1 Left
    ///  0 Center/Neutral
    /// +1 Right    
    /// </summary>
    public double X { get; private set; }

    /// <summary>
    /// Y position of the input device
    /// -1 Up
    ///  0 Center/Neutral
    /// +1 Down    
    /// </summary>
    public double Y { get; private set; }

    /// <summary>
    /// State of Button 1 as mapped by the Input provider.     
    /// </summary>
    public bool Button1 { get; set; }

    /// <summary>
    /// State of Button 2 as mapped by the Input provider.     
    /// </summary>
    public bool Button2 { get; set; }

    /// <summary>
    /// State of Button 3 as mapped by the Input provider.     
    /// </summary>
    public bool Button3 { get; set; }
    
    /// <summary>
    /// State of Button 4 as mapped by the Input provider.     
    /// </summary>
    public bool Button4 { get; set; }

    /// <summary>
    /// Creates an InputManager instance
    /// </summary>
    public InputManager()
    {      
    }

    /// <summary>
    /// Register a new input provider
    /// </summary>
    /// <param name="provider">Input provider to register</param>
    public void AddInputProvider(IInputProvider provider)
    {
      if (_count + 1 == MaxProviders) throw new Exception("Exceeded maximum number of providers");
      _providers[_count++] = provider;
    }

    /// <summary>
    /// The number of input providers registered
    /// </summary>
    public int Count { get { return _count; } }

    /// <summary>
    /// Access a specific input provider.    
    /// </summary>
    /// <param name="index">Index on the input provider to access.</param>
    /// <returns>Input provider instance.</returns>
    public IInputProvider this[int index]
    {
      get { return _providers[index]; }
    }

    internal void Update(long elapsedTime)
    {
      double xSum = 0;
      double ySum = 0;

      Button1 = false;
      Button2 = false;
      Button3 = false;
      Button4 = false;
      
      for (int i = 0; i < _count; ++i)
      {
        IInputProvider provider = _providers[i];
        
        provider.Update(elapsedTime);
        xSum += provider.X;
        ySum += provider.Y;
        Button1 = Button1 ? true : provider.Button1;
        Button2 = Button2 ? true : provider.Button2;
        Button3 = Button3 ? true : provider.Button3;
        Button4 = Button4 ? true : provider.Button4;
      }

      X = xSum / _count;
      Y = ySum / _count;
    }
  }
}
