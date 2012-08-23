// Copyright (c) 2012 Chris Taylor

using Microsoft.SPOT;
using dotnetwarrior.NetMF.Game;
using dotnetwarrior.NetMF.Diagnostics;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

namespace Pacman
{
  abstract class GameCharacter : Sprite
  {
    private Direction _currentDirection = Direction.Left;
    private int _speed = 4;

    public GameCharacter(Maze maze)
    {
      Maze = maze;
      TargetSpeed = 4;
      Active = true;
      Lives = 3;
    }
    
    protected Maze Maze { get; set; }
    protected int TargetSpeed { get; set; }

    public int Score { get; set; }
    public int Lives { get; set; }
    public bool Active { get; set; }

    public Direction TargetDirection
    {
      get;
      set;
    }

    public Direction CurrentDirection
    {
      get { return _currentDirection; }
      set
      {
        if (_currentDirection != value)
        {
          _currentDirection = value;
          TargetDirection = value;
          SetCharacterAnimation();
        }
      }
    }

    public abstract void Reset();
    protected abstract Direction OnSelectNewDirection(int xCell, int yCell);
    protected abstract void SetCharacterAnimation();
    public abstract bool CanEnterCell(byte value);

    public override void Update(long elapsedTime)
    {
      if (!Active) return;

      using (MiniProfiler.Enter("GameCharacter.Update"))
      {
        if ((X & 15) == 0 && (Y & 15) == 0)
        {
          using (MiniProfiler.Enter("GameCharacter.Update.InCell"))
          {
            int xCell = X >> 4;
            int yCell = Y >> 4;

            TargetDirection = OnSelectNewDirection(xCell, yCell);

            if (TargetDirection == Direction.Stop)
            {
              CurrentDirection = Direction.Stop;
            }
            else if (TargetDirection != _currentDirection && Maze.IsCellOpen(this, TargetDirection))
            {
              CurrentDirection = TargetDirection;            
            }
            else if (!Maze.IsCellOpen(this, CurrentDirection))
            {
              CurrentDirection = Direction.Stop;
            }
          }
        }

        if (TargetSpeed != _speed)
        {
          int modMask = TargetSpeed - 1;
          if ((X & modMask) == 0 && (Y & modMask) == 0)
          {
            _speed = TargetSpeed;
          }
        }
        Move();

        base.Update(elapsedTime);
      }
    }

    private void Move()
    {
      switch (_currentDirection)
      {
        case Direction.Left: X -= _speed; break;
        case Direction.Right: X += _speed; break;
        case Direction.Up: Y -= _speed; break;
        case Direction.Down: Y += _speed; break;
      }
    }

    public override void Draw(Bitmap surface, Picturebox host)
    {
      base.Draw(surface, host);
    }

    protected Direction GetOppositeDirection()
    {
      if (CurrentDirection == Direction.Left) return Direction.Right;
      if (CurrentDirection == Direction.Right) return Direction.Left;
      if (CurrentDirection == Direction.Up) return Direction.Down;
      if (CurrentDirection == Direction.Down) return Direction.Up;
      return Direction.Stop;
    }

    public static Direction[] Directions = new Direction[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
  }

  enum Direction
  {
    Stop = 0,

    Left = 1,
    Right = 2,
    Up = 5,
    Down = 6,
  }
}
