using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance;

    public Action<string> OnIdReceived = null;
    public Action<string, string> OnNameReceived = null;
    public Action<string,string> OnMessageReceived = null;


    private void Awake()
    {
        Instance = this;
    }

    public void ws_Onmessage(object sender, MessageEventArgs e)
    {
        var result = JsonConvert.DeserializeObject<Data>(e.Data);
        Debug.Log(result.Type);

        if (result.Type == "id")
        {
            OnIdReceived?.Invoke(result.Value[0]);
        }
        else if (result.Type == "message")
        {
            OnMessageReceived?.Invoke(result.Value[0], result.Value[1]);
        }
        else if (result.Type == "name")
        {
            OnNameReceived?.Invoke(result.Value[0], result.Value[1]);
        }
    }
   

}
