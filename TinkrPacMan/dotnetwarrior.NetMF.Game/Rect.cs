// Copyright (c) 2012 Chris Taylor

namespace dotnetwarrior.NetMF.Game
{
  /// <summary>
  /// Describes the location, width and height of a rectangle.
  /// </summary>
  public struct Rect
  {
    /// <summary>
    /// Constructs a new instance of the Rect structure
    /// </summary>
    /// <param name="x">X-coordinate of the upper left corner of the rectangle</param>
    /// <param name="y">Y-coordinate of the upper left corner of the rectangle</param>
    /// <param name="width">Width of the rectangle</param>
    /// <param name="height">Height of the rectangle</param>
    public Rect(int x, int y, int width, int height)
      : this()
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
    }

    /// <summary>
    /// X-coordinate of the upper left corner of the rectangle
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y-coordinate of the upper left corner of the rectangle
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Width of the rectangle
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Height of the rectangle
    /// </summary>
    public int Height { get; set; }    
  }
}
