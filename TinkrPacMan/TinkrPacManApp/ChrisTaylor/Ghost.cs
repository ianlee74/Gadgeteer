// Copyright (c) 2012 Chris Taylor

using System;
using Microsoft.SPOT;
using dotnetwarrior.NetMF.Game;
using dotnetwarrior.NetMF.Diagnostics;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

using Rect = dotnetwarrior.NetMF.Game.Rect;

namespace Pacman
{
  class Ghost : GameCharacter
  {
    private static Random Prng = new Random();

    #region Animation Selection Constants

    // Changes to these constants must match the order 
    // of the animation sequences defined in the ctor.
    private const int AnimateLeft = 0;
    private const int AnimateRight = 1;
    private const int AnimateUp = 2;
    private const int AnimateDown = 3;
    private const int AnimateFrightened = 4;
    private const int AnimateRecovering = 5;
    private const int AnimateDeadLeft = 6;
    private const int AnimateDeadRight = 7;
    private const int AnimateDeadUp = 8;
    private const int AnimateDeadDown = 9;

    #endregion

    private PacmanGame _game;

    private int _chaseTime = 20 * 1000;
    private int _scatterTime = 7 * 1000;
    private int _frightendTime = 6 * 1000;
    private int _warnTime = 2 * 1000;

    private GhostState _state = GhostState.Chase;
    private Personality _personality;
    protected Player Player { get; set; }

    private CountDownTimer _stateCountDown = new CountDownTimer(0);

    public Ghost(Bitmap spriteSheet, Maze maze, Player player, int spriteSheetYOffset, Personality personality)
      : base(maze)
    {
      _game = (PacmanGame)GameManager.Game;

      Player = player;
      _personality = personality;

      // NB: The sequence of the animations must match the 
      //     "Animation Selection Constants" defined above      
      SetAnimationSequences(new AnimationSequence[]
      { 
        // 0 - Left
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(0, spriteSheetYOffset, 16, 16),
          new Rect(16, spriteSheetYOffset, 16, 16),
        }),
        // 1 - Right
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(32, spriteSheetYOffset, 16, 16),
          new Rect(48, spriteSheetYOffset, 16, 16),
        }),
        // 2 - Up
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(64, spriteSheetYOffset, 16, 16),
          new Rect(80, spriteSheetYOffset, 16, 16),
        }),
        // 3 - Down
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(96, spriteSheetYOffset, 16, 16),
          new Rect(112, spriteSheetYOffset, 16, 16),
        }),        

        // 4 - Frightened
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(128, 16, 16, 16),
          new Rect(144, 16, 16, 16),
        }),

        // 5 - Recovering
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(128, 16, 16, 16),
          new Rect(128, 32, 16, 16),
          new Rect(144, 16, 16, 16),
          new Rect(144, 32, 16, 16),
        }),
        // 6 - Left Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(128, 48, 16, 16),          
        }),
        // 7 - Right Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(144, 48, 16, 16),          
        }),
        // 8 - Up Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(128, 64, 16, 16),          
        }),
        // 9 - Down Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(144, 64, 16, 16),          
        }),
      });

      Reset();
      
        
      MessageService.Instance.Subscribe(typeof(Messages.AtePowerPillMessage), HandleAtePowerPillMessage);
      MessageService.Instance.Subscribe(typeof(Messages.PacmanDyingMessage), HandlePacmanDyingMessage);
      MessageService.Instance.Subscribe(typeof(Messages.PacmanDeadMessage), HandlePacmanDeadMessage);

      _stateCountDown.Expired += StateCountDown_Expired;
    }

    #region Message subscription handlers
    private void HandleAtePowerPillMessage(object message)
    {
      if (_state != GhostState.Dead)
      {
        SetState(GhostState.Frightened);
        CurrentAnimation = AnimateFrightened;
      }
    }

    private void HandlePacmanDyingMessage(object message)
    {
      Active = false;
    }

    private void HandlePacmanDeadMessage(object message)
    {
      Reset();
    }

    private void StateCountDown_Expired(object sender, EventArgs e)
    {
      switch (_state)
      {
        case GhostState.Chase:
          SetState(GhostState.Scatter); break;
        case GhostState.Scatter:
          SetState(GhostState.Chase); break;
        case GhostState.Frightened:
          SetState(GhostState.Recovering); break;
        case GhostState.Recovering:
          SetState(GhostState.Scatter); break;
      }
    }

    #endregion

    public bool IsFrightened { get { return _state == GhostState.Frightened || _state == GhostState.Recovering; } }
    public bool IsDead { get { return _state == GhostState.Dead; } }    
    public bool IsInHome
    {
      get
      {
        int xCell = X >> 4;
        int yCell = Y >> 4;
        return (yCell == 7) && (xCell >= 6 && xCell <= 8);
      }
    }

    public void Die()
    {
      SetState(GhostState.Dead);
    }

    private int DistanceToTarget(int tx, int ty, Direction direction)
    {
      int dx = 0;
      int dy = 0;

      switch (direction)
      {
        case Direction.Left: dx = tx - (X - 1); dy = ty - Y; break;
        case Direction.Right: dx = tx - (X + 1); dy = ty - Y; break;
        case Direction.Up: dy = ty - (Y - 1); dx = tx - X; break;
        case Direction.Down: dy = ty - (Y + 1); dx = tx - X; break;
      }
      return (dx * dx) + (dy * dy);
    }

    public override void Reset()
    {
      Active = true;
      SetState(GhostState.Scatter);
      X = _personality.StartX;
      Y = _personality.StartY;

      // Increase difficulty based on the current level
      _scatterTime -= (_game.Level - 1) * 250;
      _frightendTime -= (_game.Level - 1) * 250;

      _scatterTime = System.Math.Max(_scatterTime, 150);
      _frightendTime = System.Math.Max(_scatterTime, 150);
    }

    private void SetState(GhostState state)
    {
      _state = state;
      TargetSpeed = 4;
      switch (state)
      {
        case GhostState.Scatter: _stateCountDown.Start(_scatterTime); break;
        case GhostState.Chase: _stateCountDown.Start(_chaseTime); break;
        case GhostState.Frightened:
          _stateCountDown.Start(_frightendTime);
          if (Maze.IsCellOpen(this, GetOppositeDirection()))
          {
            CurrentDirection = GetOppositeDirection();
          }
          TargetSpeed = 2;
          break;
        case GhostState.Recovering: 
          _stateCountDown.Start(_warnTime);
          TargetSpeed = 2;
          break;
        case GhostState.Dead:
          if (Maze.IsCellOpen(this, GetOppositeDirection()))
          {
            CurrentDirection = GetOppositeDirection();
          }
          break;
      }
      SetCharacterAnimation();
    }
    
    protected override Direction OnSelectNewDirection(int xCell, int yCell)
    {
      using (MiniProfiler.Enter("Ghost.OnSelectNewDirection"))
      {
        Direction newDirection = Direction.Stop;

        if (!IsDead && Prng.NextDouble() < 0.3)
        {
          using (MiniProfiler.Enter("Random"))
          {
            newDirection = Wander();
          }
        }
        else if (_state == GhostState.Scatter)
        {
          using (MiniProfiler.Enter("Scatter"))
          {
            newDirection = HuntTarget(
              _personality.ScatterX,
              _personality.ScatterY);
          }
        }
        else if (_state == GhostState.Chase)
        {
          using (MiniProfiler.Enter("Chase"))
          {
            newDirection = HuntTarget(Player);
          }
        }
        else if (IsFrightened)
        {
          newDirection = Wander();
        }
        else if (IsDead)
        {
          newDirection = HuntTarget(
            _personality.HouseX,
            _personality.HouseY);

          if (IsInHome)
          {
            SetState(GhostState.Scatter);
          }
        }

        return newDirection;
      }
    }

    #region Ghost movement strategies
    private Direction Wander()
    {
      using (MiniProfiler.Enter("Wander"))
      {
        Direction[] open;
        using (MiniProfiler.Enter("Wander.GetOpenDirections"))
        {
          open = Maze.GetOpenDirections(this, CurrentDirection);
        }        
        
        using (MiniProfiler.Enter("Wander.Pick"))
        {
          int count = 0;
          for (int i = 0; i < open.Length; ++i)
          {
            if (open[i] == Direction.Stop) break;
            count++;
          }
          return open[Prng.Next(count)];
        }
      }
    }

    private Direction HuntTarget(Player player)
    {
      int tx = player.X;
      int ty = player.Y;

      switch (player.CurrentDirection)
      {
        case Direction.Left: tx -= _personality.TargetOffsetX; break;
        case Direction.Right: tx += _personality.TargetOffsetX; break;
        case Direction.Up: ty -= _personality.TargetOffsetY; break;
        case Direction.Down: ty += _personality.TargetOffsetY; break;
      }

      return HuntTarget(tx, ty);
    }

    private Direction HuntTarget(int tx, int ty)
    {
      Direction[] open = Maze.GetOpenDirections(this, CurrentDirection);
      Direction minDirection = open[0];
      int minDistance = int.MaxValue;
      for (int i = 0; i < open.Length; ++i)
      {
        var dir = open[i];
        if (dir == Direction.Stop) break;
        int distance = DistanceToTarget(tx, ty, dir);
        if (distance < minDistance)
        {
          minDirection = dir;
          minDistance = distance;
        }
      }
      return minDirection;
    }

    private Direction EvadeTarget(int tx, int ty)
    {
      Direction[] open = Maze.GetOpenDirections(this, Direction.Stop);
      Direction maxDirection = open[0];
      int maxDistance = int.MinValue;
      for (int i = 0; i < open.Length; ++i)
      {
        var dir = open[i];
        if (dir == Direction.Stop) break;
        int distance = DistanceToTarget(tx, ty, dir);
        if (distance > maxDistance)
        {
          maxDirection = dir;
          maxDistance = distance;
        }
      }
      return maxDirection;
    }
    #endregion

    protected override void SetCharacterAnimation()
    {
      if (_state == GhostState.Frightened)
      {
        CurrentAnimation = AnimateFrightened;        
      }
      else if (_state == GhostState.Recovering)
      {
        CurrentAnimation = AnimateRecovering;
      }
      else if (IsDead)
      {
        switch (CurrentDirection)
        {
          case Direction.Left: CurrentAnimation = AnimateDeadLeft; break;
          case Direction.Right: CurrentAnimation = AnimateDeadRight; break;
          case Direction.Up: CurrentAnimation = AnimateDeadUp; break;
          case Direction.Down: CurrentAnimation = AnimateDeadDown; break;
        }
      }
      else
      {
        switch (CurrentDirection)
        {
          case Direction.Left: CurrentAnimation = AnimateLeft; break;
          case Direction.Right: CurrentAnimation = AnimateRight; break;
          case Direction.Up: CurrentAnimation = AnimateUp; break;
          case Direction.Down: CurrentAnimation = AnimateDown; break;
        }
      }      
    }    

    public override bool CanEnterCell(byte value)
    {
      // Can move through the house door when dead or already in the ghost house
      if (value == MazeCell.HouseDoor)
      {
        return (IsDead || IsInHome);
      }
      
      return value != MazeCell.Wall 
        && value != MazeCell.HouseDoor 
        && value != MazeCell.Teleport;      
    }

    public override void Update(long elapsedTime)
    {
      if (!Active) return;

      using (MiniProfiler.Enter("Ghost.Update (Active)"))
      {
        _stateCountDown.Update(elapsedTime);

        if (!IsDead)
        {
          if (Maze.IsColliding(X, Y, Player.X, Player.Y))
          {
            MessageService.Instance.Publish(Messages.CollidedWithPacmanMessage.Message(this));
            if (IsFrightened) Die();
          }
        }

        base.Update(elapsedTime);
      }
    }

    public override void Draw(Bitmap surface, Picturebox host)
    {
      if (!Active) 
          return;
      base.Draw(surface, host);
    }
  }

  class Personality
  {
    public int ScatterX { get; set; }
    public int ScatterY { get; set; }
    public int TargetOffsetX { get; set; }
    public int TargetOffsetY { get; set; }
    public int HouseX { get; set; }
    public int HouseY { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
  }

  enum GhostState
  {    
    Chase,
    Scatter,
    Frightened,
    Recovering,
    Dead
  }
}
