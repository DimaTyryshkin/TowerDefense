using UnityEngine;
using UnityEngine.Playables;

public class EventReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(
        Playable origin,
        INotification notification,
        object context)
    {
        if (notification is EventMarker marker)
        {
            Debug.Log($"Timeline event: {marker.message}");
        }
    }
}