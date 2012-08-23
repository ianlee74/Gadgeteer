// Copyright (c) 2012 Chris Taylor

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using dotnetwarrior.NetMF.Game;
using dotnetwarrior.NetMF.Diagnostics;

using TinkrPacManApp;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

using Rect = dotnetwarrior.NetMF.Game.Rect;

namespace Pacman
{
  class Maze : GameObject
  {
    private Bitmap _mazeSpriteSheet;
    private Bitmap _cache;
    private Rect[] _walls;    
    private int _dotCount;
    private int _dotsEaten;
    private BonusItemType _bonusItem = BonusItemType.None;
    private Rect _bonusItemRect;

    private byte[][] _maze = new byte[][]
    {
      new byte[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
      new byte[] {1,2,2,2,2,2,2,1,2,2,2,2,2,2,1},
      new byte[] {1,3,1,1,2,1,1,1,1,1,2,1,1,3,1},
      new byte[] {1,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
      new byte[] {1,1,1,1,2,1,1,1,1,1,2,1,1,1,1},
      new byte[] {1,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
      new byte[] {1,1,1,1,2,1,1,5,1,1,2,1,1,1,1},
      new byte[] {4,2,2,2,2,1,0,0,0,1,2,2,2,2,4},
      new byte[] {1,1,1,1,2,1,1,1,1,1,2,1,1,1,1},
      new byte[] {1,2,2,2,2,2,2,0,2,2,2,2,2,2,1},
      new byte[] {1,2,1,1,2,1,1,1,1,1,2,1,1,2,1},
      new byte[] {1,3,2,1,2,2,2,1,2,2,2,1,2,3,1},
      new byte[] {1,1,2,1,1,1,2,1,2,1,1,1,2,1,1},
      new byte[] {1,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
      new byte[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},      
    };

    private Rect[] _bonusItemRects = new Rect[]
    {
      new Rect(32,64,16,16), // Cherry
      new Rect(48,64,16,16), // Strawberry
      new Rect(64,64,16,16), // Peach
      new Rect(0,80,16,16),  // Apple
      new Rect(16,80,16,16), // Grape
      new Rect(32,80,16,16), // Galaxian
      new Rect(48,80,16,16), // Bell
      new Rect(64,80,16,16), // Key
    };

    public int DotsEaten { get { return _dotsEaten; } }
    public int BonusEaten { get; private set; }

    public BonusItemType BonusItem
    {
      get { return _bonusItem; }
      set
      {
        _bonusItem = value;
        if (_bonusItem != BonusItemType.None)
        {
          byte bonusItemIndex = (byte)_bonusItem;
          _bonusItemRect = _bonusItemRects[bonusItemIndex];
          _maze[9][7] = (byte)(MazeCell.Cherry + bonusItemIndex);
        }
        else
        {
          _maze[9][7] = MazeCell.Clear;
        }
      }
    }

    public Maze()
    {
      _mazeSpriteSheet = Resources.GetBitmap(Resources.BitmapResources.maze_walls);

      // Wall image mapping based on neighbors using a binary scheme
      // In the image below * represents the current map location, and the surrounding numbers
      // represent the bit value that used if the corresponding location has wall piece. The sum
      // of the populated neighbors represents the index into the _walls array at which the 
      // image can be found that represents the piece that integrates with this neighbor configuration.
      //     |1|
      //   |8|*|2|
      //     |4|
      // For example, the top left corner in the map has one neighbor to the right an one below
      // which means that the top left corner should be an corner piece which is open on the 
      // right and bottom, the neighbor of the right and bottom has value 2 and 4 therefore 
      // the image that integrates with these neighbors is stored at index 6 (2+4) in the wall array.
      //
      // Note: It is not the actual image, but the rect location in the sprite sheet from which the image
      //       will be extracted.

      _walls = new Rect[16];
      _walls[0] = WallToRect(18);
      _walls[1] = WallToRect(19);
      _walls[2] = WallToRect(4);
      _walls[3] = WallToRect(10);
      _walls[4] = WallToRect(14);
      _walls[5] = WallToRect(5);
      _walls[6] = WallToRect(0);
      _walls[7] = WallToRect(11);
      _walls[8] = WallToRect(9);
      _walls[9] = WallToRect(13);
      _walls[10] = WallToRect(1);
      _walls[11] = WallToRect(12);
      _walls[12] = WallToRect(3);
      _walls[13] = WallToRect(8);
      _walls[14] = WallToRect(2);
      _walls[15] = WallToRect(7);

      _cache = new Bitmap(320, 240);
      Reset();
    }

    public void Reset()
    {
      BonusItem = BonusItemType.None;
      BonusEaten = 0;
      _dotCount = 0;
      _dotsEaten = 0;

      for (int y = 0; y < _maze.Length; ++y)
      {
        for (int x = 0; x < _maze[y].Length; ++x)
        {
          byte cellValue = _maze[y][x];
          if (cellValue >= 100)
          {
            _maze[y][x] -= 100;
          }
        }
      }
      _maze[9][7] = 0;
      UpdateMazeCache();
    }

    private Rect WallToRect(int index)
    {
      int x = index % 5;
      int y = index / 5;
      return new Rect(x * 16, y * 16, 16, 16);
    }

    private int GetNeighbors(byte[][] map, int x, int y, byte value)
    {
      int result = 0;
      if (x > 0 && map[y][x - 1] == value) result |= 8;
      if (x < map.Length - 1 && map[y][x + 1] == value) result |= 2;
      if (y > 0 && map[y - 1][x] == value) result |= 1;
      if (y < map.Length - 1 && map[y + 1][x] == value) result |= 4;
      return result;
    }

    public override void Draw(Bitmap surface, Picturebox host)
    {
      surface.DrawImage(0, 0, _cache, 0, 0, 320, 240);
      if (_bonusItem != BonusItemType.None)
      {
        surface.DrawImage(112, 144, _mazeSpriteSheet, 
          _bonusItemRect.X, _bonusItemRect.Y, 
          _bonusItemRect.Width, _bonusItemRect.Height);
      }
    }

    private void UpdateMazeCache()
    {
      for (int y = 0; y < _maze.Length; ++y)
      {
        for (int x = 0; x < _maze[y].Length; ++x)
        {
          int xCell = x << 4;
          int yCell = y << 4;

          byte cellValue = _maze[y][x];
          
          if (cellValue == MazeCell.Wall)
          {
            int index = GetNeighbors(_maze, x, y, MazeCell.Wall);
            Rect wall = _walls[index];
            _cache.DrawImage(xCell, yCell, _mazeSpriteSheet, wall.X, wall.Y, 16, 16);
          }
          else if (cellValue == MazeCell.Dot)
          {
            _cache.DrawImage(xCell, yCell, _mazeSpriteSheet, 0, 48, 16, 16);
            _dotCount++;
          }
          else if (cellValue == MazeCell.Enegizer)
          {
            _cache.DrawImage(xCell, yCell, _mazeSpriteSheet, 16, 48, 16, 16);
            _dotCount++;
          }
        }
      }

      // Draw the House gate
      _cache.DrawImage(96 + 12, 96, _mazeSpriteSheet, 0, 64, 25, 16);
    }

    private Direction[] _openDirections = new Direction[4];
    public Direction[] GetOpenDirections(GameCharacter character, Direction current)
    {
      int i = 0;

      _openDirections[0] = Direction.Stop;
      _openDirections[1] = Direction.Stop;
      _openDirections[2] = Direction.Stop;
      _openDirections[3] = Direction.Stop;

      if (current != Direction.Right && IsCellOpen(character, Direction.Left)) _openDirections[i++] = Direction.Left;
      if (current != Direction.Left && IsCellOpen(character, Direction.Right)) _openDirections[i++] = Direction.Right;
      if (current != Direction.Down && IsCellOpen(character, Direction.Up)) _openDirections[i++] = Direction.Up;
      if (current != Direction.Up && IsCellOpen(character, Direction.Down)) _openDirections[i++] = Direction.Down;

      if (i == 0)
      {
        if (current == Direction.Right && IsCellOpen(character, Direction.Left)) _openDirections[i++] = Direction.Left;
        else if (current == Direction.Left && IsCellOpen(character, Direction.Right)) _openDirections[i++] = Direction.Right;
        else if (current == Direction.Down && IsCellOpen(character, Direction.Up)) _openDirections[i++] = Direction.Up;
        else if (current == Direction.Up && IsCellOpen(character, Direction.Down)) _openDirections[i++] = Direction.Down;
      }

      return _openDirections;      
    }

    public bool IsCellOpen(GameCharacter character, Direction direction)
    {
      int x = character.X >> 4;
      int y = character.Y >> 4;
      switch (direction)
      {
        case Direction.Left: return character.CanEnterCell(_maze[y][x - 1]);
        case Direction.Right: return character.CanEnterCell(_maze[y][x + 1]);
        case Direction.Up: return character.CanEnterCell(_maze[y - 1][x]);
        case Direction.Down: return character.CanEnterCell(_maze[y + 1][x]);
      }

      return false;
    }

    public byte GetCellValue(int x, int y)
    {
      int xCell = (x + 8) >> 4;
      int yCell = (y + 8) >> 4;

      return _maze[yCell][xCell];
    }

    public bool IsColliding(int x1, int y1, int x2, int y2)
    {
      // Objects are colliding if centers are within
      // 12 pixels of each other. 
      // Eliminated sqrt for performance.
      int dx = (x1 + 8) - (x2 + 8);
      int dy = (y1 + 8) - (y2 + 8);

      return ((dx * dx) + (dy * dy)) < 144;
    }

    public void ClearCell(int x, int y)
    {
      int xCell = (x + 8) >> 4;
      int yCell = (y + 8) >> 4;

      _cache.DrawRectangle(Color.Black,
        1, xCell << 4, yCell << 4, 16, 16,
        0, 0,
        Color.Black, 0, 0,
        Color.Black, 0, 0, Bitmap.OpacityOpaque);

      byte cellValue = _maze[yCell][xCell];
      switch (cellValue)
      {
        case MazeCell.Dot:
        case MazeCell.Enegizer:
          _dotCount--;
          _dotsEaten++;
          break;
        case MazeCell.Cherry:
        case MazeCell.Strawberry:
        case MazeCell.Peach:
        case MazeCell.Apple:
        case MazeCell.Grape:
        case MazeCell.Galaxian:
        case MazeCell.Bell:
        case MazeCell.Key:
          BonusEaten++;
          break;
      }
        
      

      _maze[yCell][xCell] += 100;
      
      if (_dotCount == 0)
      {
        OnLevelComplete(EventArgs.Empty);
      }
    }

    protected virtual void OnLevelComplete(EventArgs e)
    {
      if (LevelComplete != null)
      {
        LevelComplete(this, e);
      }
    }

    public event EventHandler LevelComplete;
  }
}
