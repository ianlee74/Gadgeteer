// Copyright (c) 2012 Chris Taylor

using Microsoft.SPOT;
using dotnetwarrior.NetMF.Diagnostics;
using dotnetwarrior.NetMF.Game;

using TinkrPacManApp;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

using Rect = dotnetwarrior.NetMF.Game.Rect;

namespace Pacman
{
  class PacmanGame : GameManager
  {    
    private const long ReadyDefaultCountDown = 2000;
    private const long BonusDefaultCountDown = 10000;

    private Font _font;
    private Bitmap _spriteSheet;
    private Player _pacman;
    private Ghost _blinky;
    private Ghost _pinky;
    private Ghost _inky;
    private Ghost _clyde;
    private Maze _maze;
    private CountDownTimer _readyCountDown = new CountDownTimer(ReadyDefaultCountDown);
    private CountDownTimer _showBonusCountDown = new CountDownTimer(BonusDefaultCountDown);
    private CountDownTimer _showBonusScoreCountDown = new CountDownTimer(1000);
    
    private Sprite _bonus200;
    private Sprite _bonus400;
    private Sprite _bonus800;
    private Sprite _bonus1600;

    private Sprite _bonus100;
    private Sprite _bonus300;
    private Sprite _bonus500;
    private Sprite _bonus700;
    private Sprite _bonus1000;
    private Sprite _bonus2000;
    private Sprite _bonus3000;
    private Sprite _bonus5000;
    private Sprite _currentBonusSprite;

    private bool _gameOver;

    public int Level { get; private set; }

    public PacmanGame(Bitmap surface, Picturebox host) : base(surface, host)
    {
      Level = 1;      
    }

    public override void LoadContent()
    {
      _font = Resources.GetFont(Resources.FontResources.NinaB);

      var tempSheet = Resources.GetBitmap(Resources.BitmapResources.pacman);
      _spriteSheet = new Bitmap(tempSheet.Width, tempSheet.Height);
      _spriteSheet.DrawImage(0, 0, tempSheet, 0, 0, tempSheet.Width, tempSheet.Height);
      _spriteSheet.MakeTransparent(Microsoft.SPOT.Presentation.Media.Color.Black);

      _maze = new Maze();
      _maze.Draw(Surface, Host);

      _pacman = new Player(_spriteSheet, _maze);

      #region Initialize Bonus Point Sprites
      _bonus200 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(80, 104, 16, 8) }));

      _bonus400 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(96, 104, 16, 8) }));

      _bonus800 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(112, 104, 16, 8) }));

      _bonus1600 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(128, 104, 16, 8) }));


      _bonus100 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(0, 112, 16, 8) }));

      _bonus300 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(16, 112, 16, 8) }));

      _bonus500 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(32, 112, 16, 8) }));

      _bonus700 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(48, 112, 16, 8) }));

      _bonus1000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(64, 112, 20, 8) }));

      _bonus2000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(84, 112, 20, 8) }));

      _bonus3000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(106, 112, 20, 8) }));

      _bonus5000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
        new Rect[] { new Rect(128, 112, 16, 8) }));
      #endregion

      #region Initialize Ghosts
      _blinky = new Ghost(_spriteSheet, _maze, _pacman, 16,
        new Personality()
        {
          ScatterX = 13 * 16,
          ScatterY = -2 * 16,
          TargetOffsetX = 0,
          TargetOffsetY = 0,
          HouseX = 7 * 16,
          HouseY = 7 * 16,
          StartX = 7 * 16,
          StartY = 5 * 16          
        });
      _blinky.CurrentDirection = Direction.Left;

      _pinky = new Ghost(_spriteSheet, _maze, _pacman, 32,
        new Personality()
        {
          ScatterX = 1 * 16,
          ScatterY = -2 * 16,
          TargetOffsetX = 4 * 16,
          TargetOffsetY = 0,
          HouseX = 6 * 16,
          HouseY = 7 * 16,
          StartX = 6 * 16,
          StartY = 7 * 16
        });
      _pinky.CurrentDirection = Direction.Left;

      _inky = new Ghost(_spriteSheet, _maze, _pacman, 48,
        new Personality()
        {
          ScatterX = 14 * 16,
          ScatterY = 15 * 16,
          TargetOffsetX = -3 * 16,
          TargetOffsetY = 0,
          HouseX = 7 * 16,
          HouseY = 7 * 16,
          StartX = 7 * 16,
          StartY = 7 * 16
        });
      _inky.CurrentDirection = Direction.Left;

      _clyde = new Ghost(_spriteSheet, _maze, _pacman, 64,
        new Personality()
        {
          ScatterX = 0 * 16,
          ScatterY = 15 * 16,
          TargetOffsetX = 0,
          TargetOffsetY = -2 * 16,
          HouseX = 8 * 16,
          HouseY = 7 * 16,
          StartX = 8 * 16,
          StartY = 7 * 16
        });
      _clyde.CurrentDirection = Direction.Left;
      #endregion

      _pacman.Enemies = new Ghost[]
      {
        _blinky, _pinky, _inky, _clyde
      };
      
      #region Add objects to scene      
      AddToScene(_showBonusCountDown);
      AddToScene(_showBonusScoreCountDown);
      AddToScene(_maze);
      AddToScene(_pacman);
      AddToScene(_blinky);
      AddToScene(_pinky);
      AddToScene(_inky);
      AddToScene(_clyde);
      #endregion

      _maze.LevelComplete += MazeLevelComplete;
      _showBonusCountDown.Expired += BonusCountDown_Expired;

      MessageService.Instance.Subscribe(typeof(Messages.PacmanDeadMessage), HandlePacmanDeadMessage);
      MessageService.Instance.Subscribe(typeof(Messages.PacmanAteGhostMessage), HandlePacmanAteGhostMessage);
      MessageService.Instance.Subscribe(typeof(Messages.AteBonusItemMessage), HandleAteBonusItem);

      base.LoadContent();

      Reset();
    }

    #region Message subscription handlers and events

    private void HandlePacmanDeadMessage(object message)
    {
      if (_pacman.Lives == 0)
      {
        _gameOver = true;
      }
      else if (!_gameOver) 
      {
        Reset();
      }
    }

    private void HandlePacmanAteGhostMessage(object message)
    {
      Messages.PacmanAteGhostMessage m = (Messages.PacmanAteGhostMessage)message;
      switch (m.Value)
      {
        case 200: _currentBonusSprite = _bonus200; break;
        case 400: _currentBonusSprite = _bonus400; break;
        case 800: _currentBonusSprite = _bonus800; break;
        case 1600: _currentBonusSprite = _bonus1600; break;
      }

      _currentBonusSprite.X = m.X;
      _currentBonusSprite.Y = m.Y;
      _showBonusScoreCountDown.Start();      
    }

    private void HandleAteBonusItem(object message)
    {
      Messages.AteBonusItemMessage m = (Messages.AteBonusItemMessage)message;

      switch (m.Value)
      {
        case 100: _currentBonusSprite = _bonus100; break;
        case 300: _currentBonusSprite = _bonus300; break;
        case 500: _currentBonusSprite = _bonus500; break;
        case 700: _currentBonusSprite = _bonus700; break;
        case 1000: _currentBonusSprite = _bonus1000; break;
        case 2000: _currentBonusSprite = _bonus2000; break;
        case 3000: _currentBonusSprite = _bonus3000; break;
        case 5000: _currentBonusSprite = _bonus5000; break;
      }

      _currentBonusSprite.X = m.X;
      _currentBonusSprite.Y = m.Y;

      _showBonusCountDown.Expire();
      _showBonusScoreCountDown.Start();      
    }

    private void MazeLevelComplete(object sender, EventArgs e)
    {
      Reset();
      PrepareLevel(Level + 1);      
    }

    private void BonusCountDown_Expired(object sender, EventArgs e)
    {
      _maze.BonusItem = BonusItemType.None;
    }
    #endregion

    private void Reset()
    {
      if (_gameOver)
      {
        _gameOver = false;
        _pacman.Score = 0;
        _pacman.Lives = 3;

        PrepareLevel(1);
      }

      _showBonusCountDown.Cancel();
      _showBonusScoreCountDown.Cancel();  
      _readyCountDown.Start();
      _maze.BonusItem = BonusItemType.None;
    }

    private void PrepareLevel(int newLevel)
    {
      Level = newLevel;

      _maze.Reset();
      _pacman.Reset();
      _blinky.Reset();
      _pinky.Reset();
      _inky.Reset();
      _clyde.Reset();
    }

    protected override void Update(long elapsedTime)
    {
      if (InputManager.Button1)
      {
        MiniProfiler.Snapshot();
        if (_gameOver)
        {
          Reset();
        }
      }

      if (_readyCountDown.IsRunning)
      {
        _readyCountDown.Update(elapsedTime);
      }
      else
      {
        using (MiniProfiler.Enter("PacmanGame.Update"))
        {
          if (_maze.DotsEaten + _maze.BonusEaten == 30 || _maze.DotsEaten + _maze.BonusEaten == 80)
          {
            if (Level == 1)
            {
              _maze.BonusItem = BonusItemType.Cherry;
            }
            else if (Level == 2)
            {
              _maze.BonusItem = BonusItemType.Strawberry;
            }
            else if (Level >= 3 && Level <= 4)
            {
              _maze.BonusItem = BonusItemType.Peach;
            }
            else if (Level >= 5 && Level <= 6)
            {
              _maze.BonusItem = BonusItemType.Apple;
            }
            else if (Level >= 7 && Level <= 8)
            {
              _maze.BonusItem = BonusItemType.Grape;
            }
            else if (Level >= 9 && Level <= 10)
            {
              _maze.BonusItem = BonusItemType.Galaxian;
            }
            else if (Level >= 11 && Level <= 12)
            {
              _maze.BonusItem = BonusItemType.Bell;
            }
            else if (Level >= 13)
            {
              _maze.BonusItem = BonusItemType.Key;
            }

            _showBonusCountDown.Start();
          }
          using (MiniProfiler.Enter("Game.Update"))
          {
            base.Update(elapsedTime);
          }
        }        
      }
    }

    protected override void Draw()
    {
        using (MiniProfiler.Enter("PacmanGame.Draw"))
        {
            base.Draw();

            UpdateHud(Surface, _pacman.Score);

            if (_gameOver)
            {
                // Show 'Game Over'
                Surface.DrawImage(
                  (240 - 72) / 2,
                  ((240 - 8) / 2) + 16,
                  _spriteSheet, 0, 104, 72, 8);
            }
            else if (_readyCountDown.IsRunning)
            {
                // Show 'Ready!' Image
                Surface.DrawImage(
                  (240 - 48) / 2,
                  ((240 - 8) / 2) + 16,
                  _spriteSheet, 80, 96, 48, 8);
            }
            else if (_showBonusScoreCountDown.IsRunning)
                _currentBonusSprite.Draw(Surface, Host);

        }
    }

    private void UpdateHud(Bitmap surface, int score)
    {
      int y = 10;
      int x = 244;

      surface.DrawText("Level", _font, Colors.White, x, y);
      surface.DrawText(Level.ToString(), _font, Colors.White, x, y + 20);

      y += 45;
      surface.DrawText("Score", _font, Colors.White, x, y);
      surface.DrawText(score.ToString(), _font, Colors.White, x, y + 20);

      y += 45;
      surface.DrawText("Lives", _font, Colors.White, x, y);
      for (int i = 0; i < _pacman.Lives; ++i)
        surface.DrawImage(x + (i << 4), y + 20, _spriteSheet, 32, 0, 16, 16);
    }
  }
}
