using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventCenter
{
    class EventListenerSet
    {
        internal EventListener listener;
    }

    private static Dictionary<string, EventListenerSet> eventListeners;

    private static List<string> removes__;

    public delegate void EventListener(System.Object argument);

    static EventCenter()
    {
        removes__ = new List<string>();
        eventListeners = new Dictionary<string, EventListenerSet>();
    }

    public static void Send(string _event, System.Object argument = null)
    {
        EventListenerSet listenerSet;
        if (eventListeners.TryGetValue(_event, out listenerSet))
            listenerSet.listener(argument);
    }

    public static void AddListener(string _event, EventListener _listener)
    {
        EventListenerSet listenerSet;
        if (!eventListeners.TryGetValue(_event, out listenerSet))
        {
            listenerSet = new EventListenerSet();
            eventListeners.Add(_event, listenerSet);
        }
        else
            listenerSet.listener -= _listener;
        listenerSet.listener += _listener;
    }

    public static void RemoveListener(string _event, EventListener _listener)
    {
        EventListenerSet listenerSet;
        if (eventListeners.TryGetValue(_event, out listenerSet))
            if ((listenerSet.listener -= _listener) == null)
                eventListeners.Remove(_event);
    }

    public static void RemoveListener(string _event)
    {
        eventListeners.Remove(_event);
    }

    public static void RemoveListener(EventListener _listener)
    {
        removes__.Clear();
        Dictionary<string, EventListenerSet>.Enumerator enumerator = eventListeners.GetEnumerator();
        while (enumerator.MoveNext())
            if ((enumerator.Current.Value.listener -= _listener) == null)
                removes__.Add(enumerator.Current.Key);

        int removeCount = removes__.Count;

        for(int i=0; i<removeCount; i++)
        {
            eventListeners.Remove(removes__[i]);
        }
    }

    public static void RemoveListener(System.Object target)
    {
        removes__.Clear();
        Dictionary<string, EventListenerSet>.Enumerator e = eventListeners.GetEnumerator();
        while (e.MoveNext())
        {
            System.Delegate[] targetDelegates =
               System.Array.FindAll<System.Delegate>(
                   e.Current.Value.listener.GetInvocationList(), _delgate => _delgate.Target == target);
            for (int j = 0; j < targetDelegates.Length; ++j)
                e.Current.Value.listener -= (EventListener)targetDelegates[j];
            if (e.Current.Value.listener == null)
                removes__.Add(e.Current.Key);
        }
        int c = removes__.Count;
        for (int i = 0; i < c; ++i)
            eventListeners.Remove(removes__[i]);
    }

    public static void ClearListener()
    {
        eventListeners.Clear();
    }
}
