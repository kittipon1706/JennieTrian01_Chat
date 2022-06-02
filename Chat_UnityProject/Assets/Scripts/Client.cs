using System;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;

public class Client : MonoBehaviour
{
    public static Client Instance;

    [SerializeField] public string id;
    [SerializeField] public string Name;
    private WebSocket ws;
    private MessagePooling _MessagePooling;
    private EventHandler _EventHandler;

    private UI_Controller _UI_controller;

    [System.Serializable]
    public class User
    {
        public string name;
        public string Id;
    }

    [SerializeField] public Dictionary<string, User> _AllUser = new Dictionary<string, User>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _UI_controller = UI_Controller.Instance;
        _MessagePooling = MessagePooling.Instance;
        _EventHandler = EventHandler.Instance;

        _EventHandler.OnIdReceived += Set_ClientId;
        _EventHandler.OnNameReceived += Set_User;
        _EventHandler.OnPlayerDisconnect += Dis_User;

        _UI_controller._Sendbtn.onClick.AddListener(Send_Message);
        _UI_controller._Connectbtn.onClick.AddListener(Try_Connnect);
    }

    public void Try_Connnect()
    {
        if (!string.IsNullOrEmpty(_UI_controller._MyName.text) && _UI_controller._MyName.text.Length <= 10)
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

            _UI_controller._MyInput.text = string.Empty;
            return;
        }
    }

    private void ws_Onclose(object sender, CloseEventArgs e)
    {
        Debug.Log("DISCONNECTED");
        _AllUser.Clear();
        UnityMainThreadDispatcher.Instance().Enqueue(_UI_controller.OnDisconnectServerUI());
        var message = JsonConvert.SerializeObject(new { Type = "disconnect", sender = Name, msg = id });
        ws.Send(message);
    }

    private void ws_Onopen(object sender, EventArgs e)
    {
        Debug.Log("CONNECTED");
        Send_MyName();
        UnityMainThreadDispatcher.Instance().Enqueue(_UI_controller.OnConnectServerUI());
    }

    public void Send_Message()
    {
        if (!string.IsNullOrEmpty(_UI_controller._MyInput.text))
        {
            var input = string.Empty;
            var message = string.Empty;
            if (_UI_controller._MyInput.text.Length >= 30)
            {
                input = _UI_controller._MyInput.text.Substring(0, 30);
                message = JsonConvert.SerializeObject(new { Type = "message", sender = id, msg = input });
            }
            else
            {
                message = JsonConvert.SerializeObject(new { Type = "message", sender = id, msg = _UI_controller._MyInput.text });
            }
            _UI_controller._MyInput.text = string.Empty;
            ws.Send(message);
        }
        else
        {
            _UI_controller._MyInput.text = string.Empty;
        }
    }

    private void Send_MyName()
    {
        if (!string.IsNullOrEmpty(_UI_controller._MyName.text) )
        {
            var message = JsonConvert.SerializeObject(new { Type = "name", sender = _UI_controller._MyName.text , msg = id });
            ws.Send(message);

            Name = _UI_controller._MyName.text;

            _UI_controller._MyName.text = string.Empty;

        }
       
    }

   

    private void Set_ClientId(string id)
    {
        if (this.id == string.Empty)
        {
            this.id = id;
        }
    }



    private void Set_User(string[] name,string[] id)
    {
        for (int i = 0; i < name.Length; i++)
        {
            var user = new User();
            user.name = name[i];
            user.Id = id[i];
            if (!_AllUser.ContainsKey(user.Id))
            {
                _AllUser.Add(user.Id,user);

            }
           
        }

        UnityMainThreadDispatcher.Instance().Enqueue(_UI_controller.Show_Alluser());
    }

    private void Dis_User(string name , string  Id)
    {
        _AllUser.Remove(Id);
        UnityMainThreadDispatcher.Instance().Enqueue(_UI_controller.Show_Alluser());
    }



    private void OnApplicationQuit()
    {
        var message = JsonConvert.SerializeObject(new { Type = "disconnect", sender = Name , msg = id });
        ws.Send(message);
    }

}
