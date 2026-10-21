using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigger;

    public void AddListener(UnityAction unityAction)
    {
        onTrigger?.AddListener(unityAction);
    }

    public void InvokeOnTriggerEvent()
    {
        onTrigger?.Invoke();
    }
}
