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

    public static PointerEvent PointerDownHandler = new PointerEvent();
    public static PointerEvent PointerUpHandler = new PointerEvent();

    private UdpClient client;

    // Start is called before the first frame update
    void Start()
    {
        PointerDownHandler.AddListener(OnPointerDown);
        PointerUpHandler.AddListener(OnPointerUp);
        Connect.onClick.AddListener(connectToServer);
        toggleMode.onClick.AddListener(toggleEditorMode);
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
            if (task.IsCompleted)
            {
                res = Encoding.Default.GetString(task.Result.Buffer);
            }
            if (task.IsCompleted && res.Contains("HELO OK")) Status.text = "connected!";
            else throw new Exception();
        }
        catch (Exception)
        {
            Status.text = "Failed to connect!";
        }
    } 

    private void toggleEditorMode()
    {

    }

    public void OnPointerDown(PointerEventData e)
    {
        if (e.selectedObject == null) return;
        if (e.selectedObject.tag != null) keyInput(e.selectedObject, true);
    }
    public void OnPointerUp(PointerEventData e)
    {
        if (e.selectedObject == null) return;
        if (e.selectedObject.tag != null) keyInput(e.selectedObject, false);
    }

    private void keyInput(GameObject btn, bool isKeyDown)
    {
        if (client == null)
        {
            Status.text = "Please connect to server";
            return;
        }
        JObject obj = new JObject
        {
            { "isKeyDown", isKeyDown },
            { "keyCode", btn.tag }
        };
        byte[] _str = Encoding.Default.GetBytes(obj.ToString());
        client.Send(_str, _str.Length);
        Debug.Log((isKeyDown ? "btnDown" : "btnUp") + ": " + btn.tag);
    }
}
