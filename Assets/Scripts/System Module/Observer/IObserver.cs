using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    public abstract void Notify(IObserverTarget obj, string value);
}
