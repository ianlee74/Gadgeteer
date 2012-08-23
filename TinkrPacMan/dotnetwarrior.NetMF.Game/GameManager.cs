// Copyright (c) 2012 Chris Taylor

using System;
using System.Threading;
using Microsoft.SPOT;
using dotnetwarrior.NetMF.Game.Input;

using Skewworks.Tinkr;
using Skewworks.Tinkr.Controls;

namespace dotnetwarrior.NetMF.Game
{
  /// <summary>
  /// GameManager provides the overall game and scene management functionality.
  /// Developers should derive a specialized class for 
  /// </summary>
  public class GameManager
  {
    private GameObjectContainer _gameObjects = new GameObjectContainer();
    private Timer _gameTimer;
    private long _lastTicks;

    protected Bitmap Surface { get; private set; }
    protected Picturebox Host { get; private set; }

    public InputManager InputManager { get; private set; }

    /// <summary>
    /// State of the Manager. If Enabled is false the Updates are nor executed
    /// but all drawing still takes place.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Provides global access to the GameManager instance
    /// </summary>
    public static GameManager Game { get; private set; }

    /// <summary>
    /// Construct an instance of the GameManager
    /// </summary>
    /// <param name="surface">Bitmap that the GameManager will render the scene to</param>
    public GameManager(Bitmap surface, Picturebox host)
    {
      if (surface == null) 
          throw new ArgumentNullException("surface");
      if (surface == null)
          throw new ArgumentNullException("host");

      Surface = surface;
      Host = host;
      Enabled = true;
      InputManager = new Input.InputManager();
      Game = this;
    }

    /// <summary>
    /// Initialize the GameManager.
    /// This must be called to initialize and start the GameManager.
    /// </summary>    
    public void Initialize()
    {
      LoadContent();
      _lastTicks = DateTime.Now.Ticks;
      _gameTimer = new Timer(GameTimer_Tick, null, 0, 10);
    }

    public void Terminate()
    {
        if (_gameTimer != null)
            _gameTimer.Dispose();
    }

    /// <summary>
    /// Add a GameObject to the scene.
    /// </summary>
    /// <param name="gameObject">GameObject to be added to the scene</param>
    protected void AddToScene(GameObject gameObject)
    {
      _gameObjects.Add(gameObject);
    }

    /// <summary>
    /// Overriden in derived class to initialize and load game content.
    /// </summary>
    public virtual void LoadContent()
    {
    }

    /// <summary>
    /// Update the state of the GameObjects in the scene.
    /// If Enabled is false the GameOjects are not updated.
    /// </summary>
    /// <param name="elapsedTime">Elapsed time in milliseconds since last update.</param>
    protected virtual void Update(long elapsedTime)
    {
      InputManager.Update(elapsedTime);

      if (Enabled)
      {
        for (int i = 0; i < _gameObjects.Count; ++i)
        {
          GameObject gameObject = _gameObjects[i];
          if (gameObject.Enabled)
          {
            gameObject.Update(elapsedTime);
          }
        }
      }
    }

    /// <summary>
    /// Render all the GameObjects in the scene to the Surface Bitmap
    /// </summary>    
    protected virtual void Draw()
    {
      for (int i = 0; i < _gameObjects.Count; ++i)
      {
        GameObject gameObject = _gameObjects[i];
        gameObject.Draw(Surface, Host);
      }
    }

    private void GameTimer_Tick(object state)
    {
      long now = DateTime.Now.Ticks;
      long elapsedTime = (now - _lastTicks) / TimeSpan.TicksPerMillisecond;
      _lastTicks = now;

      Update(elapsedTime);

      Draw();

      Host.Invalidate();
    }
  }
}
