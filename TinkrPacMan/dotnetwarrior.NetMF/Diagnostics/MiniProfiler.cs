// Copyright (c) 2012 Chris Taylor

using System;
using Microsoft.SPOT;

namespace dotnetwarrior.NetMF.Diagnostics
{
  /// <summary>
  /// Provides simple hierachical profiling. Showing the nesting of calls and 
  /// the time taken for child calls contributing to the parent time.
  /// </summary>
  public static class MiniProfiler
  {
    private static bool _initialized = false;
    private static int _autoSnapshotEvery;
    private static int _maxProfileRecords;
    private static int _maxNestingDepth;
    private static int _recordCount;

    private static string[] _spaces;
    private static ProfileRecord[] _regions;
    
    private static int _depth = 0;
    private static int _head = 0;
    private static int _tail = 0;

    /// <summary>
    /// Initialize the MiniProfiler
    /// </summary>
    /// <param name="autoSnapshotEvery">The number of profiler records to capture before creating a snapshot. 0 for never.</param>
    /// <param name="maxProfileRecords">Maximum number of profiler records to keep in queue.</param>
    /// <param name="maxNestingDepth">Maximum nesting depth of profiler.</param>
    public static void Initialize(int autoSnapshotEvery = 0, int maxProfileRecords = 100, int maxNestingDepth = 10)
    {
      _autoSnapshotEvery = autoSnapshotEvery;
      _maxProfileRecords = maxProfileRecords;
      _maxNestingDepth = maxNestingDepth;

      _regions = new ProfileRecord[maxProfileRecords];
      for (int i = 0; i < maxProfileRecords; ++i)
      {
        _regions[i] = new ProfileRecord();
      }

      _spaces = new string[maxNestingDepth];
      _spaces[0] = string.Empty;
      for (int i = 1; i < maxNestingDepth; ++i)
      {
        _spaces[i] = new string(' ', i * 2);
      }
      _initialized = true;
    }

    /// <summary>
    /// Enter a region to be profiled.
    /// </summary>
    /// <param name="name">Name the code region being profiled</param>
    /// <returns>An IDisposable that must be disposed when the profiled region is exited.
    /// This works nicely with the 'using' statement.
    /// </returns>    
    public static IDisposable Enter(string name)
    {
      if (!_initialized) return null;

      var region = GetNextAvailable();

      region.ExecutionTime = -1;
      region.Name = name;      
      region.Depth = _depth++;
      region.Time = DateTime.Now;

      return region;
    }

    /// <summary>
    /// Dump a snapshot of the profiler records
    /// </summary>    
    public static void Snapshot()
    {
      if (!_initialized) return;

      using (Enter("MiniProfiler.Snapshot"))
      {
        int current = _tail;
        Debug.Print("Profiler Snapshot Start");
        Debug.Print("-----------------------");
        while (current != _head)
        {
          ProfileRecord region = _regions[current];
          if (region.ExecutionTime >= 0)
          {
            Debug.Print(_spaces[region.Depth] + region.Name + " : " + (region.ExecutionTime / (float)TimeSpan.TicksPerMillisecond).ToString("f3"));
          }          
          current = (current + 1) % _maxProfileRecords;
        }
        Debug.Print("-----------------------");
        Debug.Print("Profiler Snapshot End");
        Debug.Print("");
      }
    }

    private static ProfileRecord GetNextAvailable()
    {
      var region = _regions[_head];

      _head = (_head + 1) % _maxProfileRecords;

      if (_head == _tail)
      {
        _tail = (_tail + 1) % _maxProfileRecords;
      }

      return region;
    }

    class ProfileRecord : IDisposable
    {
      public DateTime Time;
      public float ExecutionTime { get; set; }
      public string Name { get; set; }
      public int Depth { get; set; }

      public void Dispose()
      {
        long endTime = DateTime.Now.Ticks;                
        this.ExecutionTime = endTime - this.Time.Ticks;
        _depth--;
        _recordCount++;
        if ((_autoSnapshotEvery > 0) && (_recordCount % _autoSnapshotEvery == 0))
        {
          Snapshot();
        }
      }
    }
  }
}
