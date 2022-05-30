using System;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public static Client Instance;
    [SerializeField] private InputField _MyInput;
    [SerializeField] private InputField _MyName;
    [SerializeField] private Button _Sendbtn;
    [SerializeField] private Button _Connectbtn;
    [SerializeField] public string id;
    [SerializeField] public string Name;
    private WebSocket ws;
    private MessagePooling _MessagePooling;
    private EventHandler _EventHandler;

    [System.Serializable]
    public class User
    {
        public string name;
        public string Id;
    }

    [SerializeField] public List<User> _AllUser = new List<User>();

    private void Start()
    {
        Instance = this;

        _EventHandler = new EventHandler();
        _EventHandler.OnIdReceived += Set_ClientId;
        _EventHandler.OnMessageReceived += Show_Message;
        _EventHandler.OnNameReceived += Set_Name;

        _MessagePooling = MessagePooling.Instance;
        _Sendbtn.onClick.AddListener(Send_Message);


        _Connectbtn.onClick.AddListener(Try_Conrrect);

    }

    private void Try_Conrrect()
    {
        if (!string.IsNullOrEmpty(_MyName.text))
        {
            try
            {
                ws = new WebSocket("ws://localhost:5000");
                ws.OnOpen += ws_Onopen;
                ws.OnMessage += _EventHandler.ws_Onmessage;
                ws.OnClose += ws_Onclose;
                ws.Connect();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        else
        {
            return;
        }
    }

    private void ws_Onclose(object sender, CloseEventArgs e)
    {
        Debug.Log("DISCONNECTED");
    }

    private void ws_Onopen(object sender, EventArgs e)
    {
        Debug.Log("CONNECTED");
        Send_Name();
        OnConnectServer();
    }

    private void Send_Message()
    {
        if (!string.IsNullOrEmpty(_MyInput.text))
        {
            var message = JsonConvert.SerializeObject(new { Type = "message", sender = id, msg = _MyInput.text });
            _MyInput.text = string.Empty;
            ws.Send(message);
        }
        
    }

    private void Send_Name()
    {
        if (!string.IsNullOrEmpty(_MyName.text))
        {
            var message = JsonConvert.SerializeObject(new { Type = "name", sender = _MyName.text , msg = id });
            ws.Send(message);

            Name = _MyInput.text;

            _MyInput.text = string.Empty;

        }
    }

    private void OnConnectServer()
    {
        _MyName.DeactivateInputField();
        _Connectbtn.enabled = false;
        _MyInput.ActivateInputField();
        _Sendbtn.enabled = true;
    }


    private void Set_ClientId(string id)
    {
        if (this.id == string.Empty)
        {
            this.id = id;
        }
    }

    private void Show_Message(string sender, string msg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(_MessagePooling.Enqueue_OutPoolmsg(sender, msg));
    }

    private void Set_Name(string name ,string Id)
    {
        var user = new User();
        user.name = name;
        user.Id = Id;
        _AllUser.Add(user);

        Show_Alluser();
    }

    private void Show_Alluser()
    {
        foreach (var item in _AllUser)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(_MessagePooling.Enqueue_OutPooluser(item.name));
        }
    }
}
