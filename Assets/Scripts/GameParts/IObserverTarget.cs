using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserverTarget
{
    public void AddObserver(List<IObserver> observerList, IObserver obj)
    {
        if (!observerList.Contains(obj))
            observerList.Add(obj);
    }

    public void RemoveObserver(List<IObserver> observerList, IObserver obj)
    {
        if (observerList.Contains(obj))
        {
            observerList.Remove(obj);
        }
    }

    public void NotifyToObservers(List<IObserver> observerList, string value)
    {
        foreach (IObserver obj in observerList)
        {
            obj.Notify(this, value);
        }
    }
}
