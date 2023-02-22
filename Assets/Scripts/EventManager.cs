using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventParameters;
using EventEnums;

public class EventManager : MonoBehaviour
{
    private static EventManager eventManager;
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType<EventManager>();
                if (!eventManager)
                {
                    Debug.LogError("there needs to be an EventManager script in the scene.");
                } 
                else
                {
                    eventManager.Initialize();
                }
            }
            return eventManager;
        }
    }
    
    private Dictionary<GameEventType, Action<EventParam>> eventDictionary;

    private void Initialize()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<GameEventType, Action<EventParam>>();
        }
    }

    public static void StartListening (GameEventType eventType, Action<EventParam> listener)
    {
        if (instance.eventDictionary.ContainsKey(eventType))
        {
            instance.eventDictionary[eventType] += listener;
        } 
        else
        {
            instance.eventDictionary.Add(eventType, listener);
        }
    }

    public static void StopListening(GameEventType eventType, Action<EventParam> listener)
    {
        if (eventManager == null) return;
        if (instance.eventDictionary.ContainsKey(eventType))
        {
            instance.eventDictionary[eventType] -= listener;
        }
    }

    public static void TriggerEvent(GameEventType eventType, EventParam param)
    {
        Action<EventParam> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.Invoke(param);
        }
    }
}
