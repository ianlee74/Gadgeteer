// Copyright (c) 2012 Chris Taylor

using Microsoft.SPOT;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

namespace dotnetwarrior.NetMF.Game
{
  /// <summary>
  /// AnimationSequence manages the animation of a sequence of
  /// images extracted from a sprite sheet.
  /// </summary>
  public class AnimationSequence
  {
    
    private Bitmap _texture;
    private Rect[] _frames;
    private int _currentFrame = 0;
    private int _frameInc = 1;
    private bool _autoReverse = true;
    private int _iterations = -1;
    private int _currentIteration = 0;
    private int _animationPeriod = 1;
    private long _updatePeriod = 0;

    /// <summary>
    /// Completed event is fired when the animation sequence completes.
    /// The event does not fire for perpetual animations
    /// </summary>
    public event EventHandler Completed;

    public AnimationSequence(Bitmap texture, int framesPerSecond, params Rect[] frames) :
      this(texture,framesPerSecond,true,-1,frames)
    {
    }

    /// <summary>
    /// Creates an instance of an AnimationSequence
    /// </summary>
    /// <param name="texture">The Bitmap which contains the sprite sheet</param>
    /// <param name="framesPerSecond">Target frame rate for the animation</param>
    /// <param name="autoReverse">If true the animation sequence is run in reverse when the end is reached</param>
    /// <param name="iterations">The number of times to run the animation. -1 Runs the animation perpetually</param>
    /// <param name="frames">List of the rectangles used to extract each frame from the sprite sheet</param>
    public AnimationSequence(Bitmap texture, int framesPerSecond, bool autoReverse, int iterations, params Rect[] frames)
    {
      _texture = texture;
      _autoReverse = autoReverse;
      _iterations = iterations * (_autoReverse ? 2 : 1);
      if (framesPerSecond > 0)
      {
        _animationPeriod = 1000 / framesPerSecond;
      }
      _frames = frames;      
    }

    /// <summary>
    /// Update the animation
    /// </summary>
    /// <param name="elapsedTime">The time that has passed since the last update</param>
    public void Update(long elapsedTime)
    {
      _updatePeriod += elapsedTime;

      if (_frames.Length == 1) return;

      if (_updatePeriod >= _animationPeriod)
      {
        _updatePeriod -= _animationPeriod;
        
        _currentFrame += _frameInc;        
        if (_currentFrame == -1 || _currentFrame == _frames.Length)
        {
          if (_autoReverse)
          {
            _frameInc = -_frameInc;
            _currentFrame += _frameInc;
          }
          else
          {
            _currentFrame = 0;
          }

          _currentIteration++;
          if (_iterations == -1 || _currentIteration == _iterations)
          {
            _currentIteration = 0;
            OnCompleted(EventArgs.Empty);
          }
        }        
      }
    }

    /// <summary>
    /// Render the current animation frame to a Bitmap
    /// </summary>
    /// <param name="surface">Bitmap to render the frame onto</param>
    /// <param name="x">X location to render the frame</param>
    /// <param name="y">Y location to render the frame</param>
    public void Draw(Bitmap surface, Picturebox host, int x, int y)
    {
      Draw(surface, host, x, y, _currentFrame);
    }

    private void Draw(Bitmap surface, Picturebox host, int x, int y, int index)
    {
      Rect r = _frames[index];
      surface.DrawImage(x, y, _texture, r.X, r.Y, r.Width, r.Height);
    }

    /// <summary>
    /// Raise the Completed event if there are registerd handlers
    /// </summary>
    /// <param name="e"></param>
    protected void OnCompleted(EventArgs e)
    {
      if (Completed != null)
      {
        Completed(this, e);
      }
    }
  }
}
