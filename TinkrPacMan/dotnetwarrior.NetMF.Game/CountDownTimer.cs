// Copyright (c) 2012 Chris Taylor

using Microsoft.SPOT;

namespace dotnetwarrior.NetMF.Game
{
  /// <summary>
  /// Provides a count down timer which generates an event when
  /// the count down expires. 
  /// </summary>
  public class CountDownTimer : GameObject
  {
    private long _timer;
    private long _duration;
    
    /// <summary>
    /// Returns true if the count down is running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Create a CountDownTimer instance with a default count down time.
    /// </summary>
    /// <param name="millseconds">Default count down time</param>
    public CountDownTimer(long millseconds)
    {
      _duration = millseconds;
      IsRunning = false;  
    }

    /// <summary>
    /// Starts the count down using the default count down time.
    /// </summary>
    public void Start()
    {
      Start(_duration);
    }

    /// <summary>
    /// Starts the count down using a custom count down time.
    /// </summary>
    /// <param name="milliseconds"></param>
    public void Start(long milliseconds)
    {
      _timer = milliseconds;
      IsRunning = true;
    }

    /// <summary>
    /// Cancels the count down without further notification.
    /// </summary>
    public void Cancel()
    {
      _timer = 0;
      IsRunning = false;
    }

    /// <summary>
    /// Expires the count down and raises the Expired event.
    /// </summary>
    public void Expire()
    {
      if (IsRunning)
      {
        Cancel();
        OnExpired();
      }
    }

    /// <summary>
    /// Update the count down timer. This must be called for
    /// the timer to count down.
    /// </summary>
    /// <param name="elapsedTime"></param>
    public override void Update(long elapsedTime)
    {
      if (!IsRunning) return;

      _timer -= elapsedTime;
      if (_timer <= 0)
      {
        Expire();
      }
    }

    protected void OnExpired()
    {
      Expired(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event that is raised when the count down completes.
    /// </summary>
    public event EventHandler Expired = delegate { };
  }
}
