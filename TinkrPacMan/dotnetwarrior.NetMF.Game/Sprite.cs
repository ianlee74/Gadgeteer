// Copyright (c) 2012 Chris Taylor

using System;
using Microsoft.SPOT;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

namespace dotnetwarrior.NetMF.Game
{
  /// <summary>
  /// Sprite GameObject manages 1 or more Animation sequences. 
  /// </summary>
  public class Sprite : GameObject
  {
    private AnimationSequence[] _animationSequences;
    
    protected void SetAnimationSequences(AnimationSequence[] animationSequences)
    {
      if (animationSequences == null) throw new ArgumentNullException("animationSequences");

      _animationSequences = animationSequences;
    }

    public int CurrentAnimation { get; set; }

    public Sprite(params AnimationSequence[] animationSequences)
    {
      if (animationSequences == null) throw new ArgumentNullException("animationSequences");
      SetAnimationSequences(animationSequences);
    }

    public override void Update(long elapsedTime)
    {
      _animationSequences[CurrentAnimation].Update(elapsedTime);
    }

    public override void Draw(Bitmap surface, Picturebox host)
    {
      _animationSequences[CurrentAnimation].Draw(surface, host, X, Y);
    }

    public AnimationSequence this[int index]
    {
      get { return _animationSequences[index]; }
    }
  }
}
