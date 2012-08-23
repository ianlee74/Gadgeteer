// Copyright (c) 2012 Chris Taylor

namespace dotnetwarrior.NetMF.Game.Input
{
  /// <summary>
  /// Defines the interface that must be implemented by custom Input Providers.
  /// </summary>
  public interface IInputProvider
  {
    /// <summary>
    /// Implementing providers must return -1 Left, 0 Center, 1 Right
    /// </summary>
    double X { get;}

    /// <summary>
    /// Implementing providers must return -1 Up, 0 Center, 1 Down
    /// </summary>
    double Y { get;}

    /// <summary>
    /// Implementing providers must return true if button 1 of the provider is pressed
    /// </summary>
    bool Button1 { get; }

    /// <summary>
    /// Implementing providers must return true if button 2 of the provider is pressed
    /// </summary>
    bool Button2 { get; }

    /// <summary>
    /// Implementing providers must return true if button 3 of the provider is pressed
    /// </summary>
    bool Button3 { get; }
    
    /// <summary>
    /// Implementing providers must return true if button 4 of the provider is pressed
    /// </summary>
    bool Button4 { get; }

    /// <summary>
    /// Called for each frame to update state of the input provider
    /// </summary>
    /// <param name="elapsedTime"></param>
    void Update(long elapsedTime);
  }
}
