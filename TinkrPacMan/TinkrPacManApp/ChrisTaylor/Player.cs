// Copyright (c) 2012 Chris Taylor

using Microsoft.SPOT;
using dotnetwarrior.NetMF.Game;

namespace Pacman
{
  class Player : GameCharacter
  {
    #region Animation Selection Constants

    // Changes to these constants must match the order 
    // of the animation sequences defined in the ctor.
    private const int AnimateLeft = 0;
    private const int AnimateRight = 1;
    private const int AnimateUp = 2;
    private const int AnimateDown = 3;
    private const int AnimateDying = 4;
    private const int AnimateStop = 5;

    #endregion

    private int _ghostKillScore = 200;
    private bool _dying = false;

    public Ghost[] Enemies { get; set; }

    public Player(Bitmap spriteSheet, Maze maze) 
      : base(maze)
    {
      // NB: The sequence of the animations must match the 
      //     "Animation Selection Constants" defined above      
      SetAnimationSequences(
        new AnimationSequence[]
        {
          // 0 - Left
          new AnimationSequence(spriteSheet, 12,
            new Rect[]
            {
              new Rect(128, 0, 16, 16), // Closed
              new Rect(0, 0, 16, 16),  // Opening
              new Rect(16, 0, 16, 16),  // Open      
            }),
          // 1 - Right
          new AnimationSequence(spriteSheet, 12,
            new Rect[]
            {
              new Rect(128, 0, 16, 16), // Closed
              new Rect(32, 0, 16, 16),  // Opening
              new Rect(48, 0, 16, 16),  // Open      
            }),
          // 2 - Up
          new AnimationSequence(spriteSheet, 12,
            new Rect[]
            {
              new Rect(128, 0, 16, 16), // Closed
              new Rect(64, 0, 16, 16),  // Opening
              new Rect(80, 0, 16, 16),  // Open      
            }),
          // 3 - Down
          new AnimationSequence(spriteSheet, 12,
            new Rect[]
            {
              new Rect(128, 0, 16, 16), // Closed
              new Rect(96, 0, 16, 16),  // Opening
              new Rect(112, 0, 16, 16),  // Open      
            }),         
          // 4 - Dying
          new AnimationSequence(spriteSheet, 12, false, -1,
            new Rect[]
            {
              new Rect(128, 0, 16, 16),
              new Rect(64, 0, 16, 16),
              new Rect(80, 0, 16, 16),
              new Rect(0, 80, 16, 16),
              new Rect(16, 80, 16, 16),
              new Rect(32, 80, 16, 16),
              new Rect(48, 80, 16, 16),
              new Rect(64, 80, 16, 16),
              new Rect(80, 80, 16, 16),
              new Rect(96, 80, 16, 16),
              new Rect(112, 80, 16, 16),
              new Rect(128, 80, 16, 16),
              new Rect(144, 80, 16, 16),
            }),
          // 5 - Stop
          new AnimationSequence(spriteSheet, 12, false, -1,
            new Rect[]
            {
              new Rect(128, 0, 16, 16),
            })         
        });

      this[AnimateDying].Completed += new EventHandler(Dying_Completed);
      
      MessageService.Instance.Subscribe(typeof(Messages.CollidedWithPacmanMessage), HandleCollidedWithPacman);

      Reset();
    }

    #region Message subscription handlers
    private void HandleCollidedWithPacman(object message)
    {
      Messages.CollidedWithPacmanMessage m = message as Messages.CollidedWithPacmanMessage;
      var ghost = m.Ghost;

      if (ghost.IsFrightened)
      {
        Score += _ghostKillScore;
        MessageService.Instance.Publish(Messages.PacmanAteGhostMessage.Message(X, Y, _ghostKillScore));
        _ghostKillScore *= 2;        
      }
      else
      {
        Die();        
      }
    }
    #endregion

    void Dying_Completed(object sender, EventArgs e)
    {
      MessageService.Instance.Publish(Messages.PacmanDeadMessage.Message());
      Reset();
    }    

    public override void Reset()
    {
      _dying = false;
      X = 7 << 4;
      Y = 9 << 4;
      CurrentAnimation = AnimateStop;
      CurrentDirection = Direction.Stop;
      TargetDirection = Direction.Stop;
    }

    protected override Direction OnSelectNewDirection(int xCell, int yCell)
    {
      return TargetDirection;
    }

    protected override void SetCharacterAnimation()
    {
      switch (CurrentDirection)
      {
        case Direction.Left: CurrentAnimation = AnimateLeft; break;
        case Direction.Right: CurrentAnimation = AnimateRight; break;
        case Direction.Up: CurrentAnimation = AnimateUp; break;
        case Direction.Down: CurrentAnimation = AnimateDown; break;
        case Direction.Stop: CurrentAnimation = AnimateStop; break;
      }
    }

    public override bool CanEnterCell(byte value)
    {
      return value != MazeCell.Wall
        && value != MazeCell.HouseDoor;
    }

    public override void Update(long elapsedTime)
    {
      if (!_dying && Lives > 0)
      {
        if (GameManager.Game.InputManager.X > 0.5)
        {
          TargetDirection = Direction.Right;
        }
        else if (GameManager.Game.InputManager.X < -0.5)
        {
          TargetDirection = Direction.Left;
        }
        else if (GameManager.Game.InputManager.Y > 0.5)
        {
          TargetDirection = Direction.Up;
        }
        else if (GameManager.Game.InputManager.Y < -0.5)
        {
          TargetDirection = Direction.Down;
        }

        byte cellValue = Maze.GetCellValue(X, Y);
        switch (cellValue)
        {
          case MazeCell.Dot :
            EatCell(10);
            break;

          case MazeCell.Enegizer:
            EatCell(50);
            FrightenEnemies();
            break;

          case MazeCell.Cherry:
            EatCell(100);
            ClearBonus(100);
            break;

          case MazeCell.Strawberry:
            EatCell(300);
            ClearBonus(300);
            break;

          case MazeCell.Peach:
            EatCell(500);
            ClearBonus(500);
            break;

          case MazeCell.Apple:
            EatCell(700);
            ClearBonus(700);
            break;
          
          case MazeCell.Grape:
            EatCell(1000);
            ClearBonus(1000);
            break;

          case MazeCell.Galaxian:
            EatCell(2000);
            ClearBonus(2000);
            break;

          case MazeCell.Bell:
            EatCell(3000);
            ClearBonus(3000);
            break;

          case MazeCell.Key:
            EatCell(5000);
            ClearBonus(5000);
            break;

          case MazeCell.Teleport:
            if (X < 20)
              {
                X = 208;
              }
              else
              {
                X = 16;
              }
            break;
        }               
      }

      base.Update(elapsedTime);
    }

    private void EatCell(int scoreValue)
    {
      Score += scoreValue;
      Maze.ClearCell(X, Y);
    }

    private void Die()
    {
      _dying = true;
      MessageService.Instance.Publish(Messages.PacmanDyingMessage.Message());
      CurrentDirection = TargetDirection = Direction.Stop;
      CurrentAnimation = AnimateDying;
      Lives--;
    }

    private void FrightenEnemies()
    {
      MessageService.Instance.Publish(Messages.AtePowerPillMessage.Message());
      _ghostKillScore = 200;
    }

    private void ClearBonus(int value)
    {
      MessageService.Instance.Publish(Messages.AteBonusItemMessage.Message(X, Y, value));
    }
  }
}
