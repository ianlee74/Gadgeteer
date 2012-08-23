// Copyright (c) 2012 Chris Taylor

using Microsoft.SPOT;
using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

namespace dotnetwarrior.NetMF.Game
{
  /// <summary>
  /// Abstract base class for all objects that can be added to a GameManager scene.
  /// </summary>
  public abstract class GameObject
  {
    /// <summary>
    /// Default constructor for GameObject.
    /// </summary>
    public GameObject()
    {
      Enabled = true;
    }

    /// <summary>
    /// Enabled controls if the object is enabled or not. 
    /// If not enabled the object is not updated, but is still rendered to the scene
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// X Location of the GameObject on the target Bitmap
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y Location of the GameObject on the target Bitmap
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Update the state of the GameObject.
    /// If Enabled is false Update is not called.
    /// Must be overriden in derived class.
    /// </summary>
    /// <param name="elapsedTime">Elapsed time in milliseconds since last update.</param>
    public virtual void Update(long elapsedTime)
    {
    }

    /// <summary>
    /// Render the GameObject to the surface Bitmap
    /// </summary>
    /// <param name="surface">Bitmap on which the GameObject will be rendered</param>
    public virtual void Draw(Bitmap surface, Picturebox host)
    {
    }
  }
}
