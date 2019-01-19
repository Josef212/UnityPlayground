using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private Dictionary<EventType, List<EventListener>> m_eventListeners = new Dictionary<EventType, List<EventListener>>();
    private Queue<KeyValuePair<EventType, Response>> m_eventQueue = new Queue<KeyValuePair<EventType, Response>>();

    public EventManager()
    {
        foreach(var type in EnumUtils<EventType>.GetAllValues())
        {
            if(!m_eventListeners.ContainsKey(type))
            {
                m_eventListeners[type] = new List<EventListener>();
            }
        }
    }

    public void AddEventListner(EventListener listener)
    {
        if (listener == null)
        {
            Debug.LogError("Error setting listner: Invalid listener.");
            return;
        }

        EventType supported = listener.GetSupportedEvents();
        foreach(var type in EnumUtils<EventType>.GetAllValues())
        {
            if((type & supported) == type)
            {
                m_eventListeners[type].Add(listener);
            }
        }
    }

    public void RemoveEventListner(EventListener listener)
    {
        if (listener == null)
        {
            return;
        }
        
        EventType supported = listener.GetSupportedEvents();
        foreach (var type in EnumUtils<EventType>.GetAllValues())
        {
            if ((type & supported) == type)
            {
                m_eventListeners[type].Remove(listener);
            }
        }
    }

    public void FireEvent(EventType type, Response response)
    {
        var listeners = m_eventListeners[type];
        foreach(var listener in listeners)
        {
            listener.RecieveEvent(type, response);
        }
    }

    public void QueueEvent(EventType type, Response response)
    {
        m_eventQueue.Enqueue(new KeyValuePair<EventType, Response>(type, response));
    }

    public void FireOneQueuedEvent()
    {
        var ev = m_eventQueue.Dequeue();
        FireEvent(ev.Key, ev.Value);
    }

    public void FireAllQueuedEvents()
    {
        while(m_eventQueue.Count != 0)
        {
            FireOneQueuedEvent();
        }
    }
}
