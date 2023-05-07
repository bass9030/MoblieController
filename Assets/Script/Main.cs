using System;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Text;

[Serializable]
public class PointerEvent : UnityEngine.Events.UnityEvent<PointerEventData> { };

public class Main : MonoBehaviour
{
    public Button Connect;
    public TMP_InputField serverAddr;
    public Button toggleMode;
    public TMP_Text Status;
    public Slider zoomSlide;

    public static PointerEvent PointerDownHandler = new PointerEvent();
    public static PointerEvent PointerUpHandler = new PointerEvent();
    public static PointerEvent DragHandler = new PointerEvent();
    public static PointerEvent DragEndHandler = new PointerEvent();

    private GameObject selectedObj;

    private bool isEditorMode = false;
    
    private UdpClient client;

    private JObject keyState = new JObject();

    // Start is called before the first frame update
    void Start()
    {
        PointerDownHandler.AddListener(OnPointerDown);
        PointerUpHandler.AddListener(OnPointerUp);
        DragHandler.AddListener(OnDrag);
        DragEndHandler.AddListener(OnEndDrag);
        Connect.onClick.AddListener(connectToServer);
        toggleMode.onClick.AddListener(toggleEditorMode);
        zoomSlide.onValueChanged.AddListener(onValueChange);
        zoomSlide.enabled = false;
    }

    private void connectToServer()
    {
        string addr = serverAddr.text;
        Status.text = "Try to connect...";
        try
        {
            client = new UdpClient();
            client.Connect(IPAddress.Parse(addr), 7777);
            byte[] _str = Encoding.Default.GetBytes("HELO");
            Status.text = "Checking connection...";
            client.Send(_str, _str.Length);
            var task = client.ReceiveAsync();
            task.Wait(5 * 1000);
            string res = "";
            if (task.IsCompleted) res = Encoding.Default.GetString(task.Result.Buffer);
            if (task.IsCompleted && res.Contains("HELO OK"))
            {
                Status.text = "connected!";
            }
            else throw new Exception();
        }
        catch (Exception)
        {
            Status.text = "Failed to connect!";
        }
    } 

    private void toggleEditorMode()
    {
        if(isEditorMode)
        {
            isEditorMode = false;
            toggleMode.gameObject.GetComponentInChildren<TMP_Text>().text = "Toggle to\r\neditor mode";
        }
        else
        {
            isEditorMode = true;
            toggleMode.gameObject.GetComponentInChildren<TMP_Text>().text = "Toggle to\r\nnormal mode";
        }
    }

    private void onValueChange(float value)
    {
        if(isEditorMode && selectedObj != null)
        {
            selectedObj.transform.localScale = new Vector3(value, value, value);
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (e.selectedObject == null)
        {
            zoomSlide.enabled = false;
            selectedObj = null;
            return;
        }
        if (e.selectedObject.tag == null)
        {
            zoomSlide.enabled = false;
            selectedObj = null;
            return;
        }
        if (isEditorMode)
        {
            zoomSlide.enabled = true;
            selectedObj = e.selectedObject;
            zoomSlide.value = selectedObj.transform.localScale.x;
        }
        else keyInput(e.selectedObject, true);
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (e.selectedObject == null) return;
        if (e.selectedObject.tag == null) return;
        if (isEditorMode) return;
        else keyInput(e.selectedObject, false);
    }

    private void OnDrag(PointerEventData e)
    {
        if (!isEditorMode) return;
        if(e.selectedObject == null) return;
        e.selectedObject.transform.position = e.position;
    }

    private void OnEndDrag(PointerEventData e)
    {
    }

    private void keyInput(GameObject btn, bool isKeyDown)
    {
        if (client == null) return;
        keyState[btn.tag] = isKeyDown;
        byte[] _str = Encoding.Default.GetBytes(keyState.ToString());
        client.Send(_str, _str.Length);
        //Debug.Log((isKeyDown ? "btnDown" : "btnUp") + ": " + btn.tag);
    }
}
