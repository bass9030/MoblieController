using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerClickInvoker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PointerEvent PointerDownHandler = new PointerEvent();
    public PointerEvent PointerUpHandler = new PointerEvent();
    public void OnPointerDown(PointerEventData e)
    {
        PointerEvent handler = Main.PointerDownHandler;
        if(handler != null) handler.Invoke(e);
    }
    public void OnPointerUp(PointerEventData e)
    {
        PointerEvent handler = Main.PointerUpHandler;
        if(handler != null) handler.Invoke(e);
    }

}
