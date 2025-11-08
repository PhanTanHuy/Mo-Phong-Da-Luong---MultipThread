using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class InteractAble : MonoBehaviour
{
    public UnityEvent eventOnInteract;
    public abstract void Interact(Transform actor);
    public void BaseInteract(Transform actor)
    {
        eventOnInteract?.Invoke();
        Interact(actor);
    }

}
