using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class EventMarker : Marker, INotification
{
    public PropertyName id => new PropertyName("Event");

    public ExposedReference<GameObject> target;
    public string message;
    public GameObject go;
}
