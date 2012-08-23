// Copyright (c) 2012 Chris Taylor

using System;
using dotnetwarrior.NetMF.Diagnostics;

namespace dotnetwarrior.NetMF.Game
{
  /// <summary>
  /// MessageService class provides pub/sub messaging infrastructure.
  /// Used for publishing notifications of occurences within the system.
  /// 
  /// Using this instead of tradditional events decouples the various components from each other
  /// to the extent that each component is only aware of the messages it is interested in an not 
  /// who is publising the message at any point in time.  
  /// 
  /// Note: Currently this is not designed for high-volume messages. But is fine for ad-hoc in game
  /// notifications.
  /// </summary>
  public class MessageService
  {    
    private static MessageService _instance = new MessageService();
    public static MessageService Instance { get { return _instance; } }

    private SubscriptionContainer _subscriptions = new SubscriptionContainer();

    /// <summary>
    /// Subscribe to a message type
    /// </summary>
    /// <param name="messageType">Type of message to subscribe to</param>
    /// <param name="handler">Handler method to be called when a message of messageType arrives</param>
    /// <returns>Opaque subscription token used to identify the subscription</returns>
    public object Subscribe(Type messageType, MessageHandler handler)
    {
      Subscription subscription = new Subscription(messageType, handler);
      _subscriptions.Add(subscription);
      return subscription;
    }

    /// <summary>
    /// Unsubscribe from an exsiting subscription.
    /// 
    /// Not yet implemented.
    /// </summary>
    /// <param name="subscriptionToken">Subscription token returned from the original call to Subscribe.</param>
    public void Unsubscribe(object subscriptionToken)
    {
      // Will implement when needed, have to think about the optimizations
      throw new NotImplementedException();
    }

    /// <summary>
    /// Publish a message for delivery to intended subscribers.
    /// </summary>
    /// <param name="message">Message to be published.</param>
    public void Publish(object message)
    {
      using (MiniProfiler.Enter("MessageService.Publish"))
      {
        Type messageType = message.GetType();
        for (int i = 0; i < _subscriptions.Count; ++i)
        {
          Subscription subscription = _subscriptions[i];
          if (messageType == subscription.MessageType)
          {
            subscription.Handler(message);
          }
        }
      }
    }

    class Subscription
    {
      public Type MessageType { get; private set; }
      public MessageHandler Handler { get; private set; }

      public Subscription(Type messageType, MessageHandler handler)
      {
        MessageType = messageType;
        Handler = handler;
      }
    }

    class SubscriptionContainer
    {
      private const int DefaultSize = 4;

      private Subscription[] _items = new Subscription[DefaultSize];
      private int _count;

      public void Add(Subscription item)
      {
        if (_count + 1 == _items.Length)
        {
          EnsureCapacity(_count + 1);
        }
        _items[_count++] = item;
      }

      public int Count { get { return _count; } }

      public Subscription this[int index]
      {
        get { return _items[index]; }
      }

      private void EnsureCapacity(int required)
      {
        if (_items == null)
        {
          _items = new Subscription[DefaultSize];
        }
        else if (required >= _items.Length)
        {
          Subscription[] newItems = new Subscription[_items.Length * 2];
          Array.Copy(_items, newItems, _items.Length);
          _items = newItems;
        }
      }
    }

    private MessageService() { }
  }

  /// <summary>
  /// Represents the method that will handle messages that the implementer has subscribed to.
  /// </summary>
  /// <param name="message"></param>
  public delegate void MessageHandler(object message);
}
