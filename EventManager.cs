using System;
using System.Collections.Generic;

public static class EventManager
{
    private static Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();
    private static Dictionary<string, Action<string>> eventDictionaryWithParam = new Dictionary<string, Action<string>>();

    public static void StartListening(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent?.Invoke();
        }
    }

    public static void StartListening(string eventName, Action<string> listener)
    {
        if (eventDictionaryWithParam.TryGetValue(eventName, out Action<string> thisEvent))
        {
            thisEvent += listener;
            eventDictionaryWithParam[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventDictionaryWithParam.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<string> listener)
    {
        if (eventDictionaryWithParam.TryGetValue(eventName, out Action<string> thisEvent))
        {
            thisEvent -= listener;
            eventDictionaryWithParam[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, string param)
    {
        if (eventDictionaryWithParam.TryGetValue(eventName, out Action<string> thisEvent))
        {
            thisEvent?.Invoke(param);
        }
    }
}