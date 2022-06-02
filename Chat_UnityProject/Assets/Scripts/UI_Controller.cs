using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    public static UI_Controller Instance;

    private MessagePooling _MessagePooling;
    private Client _Client;
    private EventHandler _EventHandler;

    [SerializeField] public InputField _MyInput;
    [SerializeField] public InputField _MyName;
    [SerializeField] public Button _Sendbtn;
    [SerializeField] public Button _Connectbtn;
    [SerializeField] private Button Exit_btn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _Client = Client.Instance;
        _MessagePooling = MessagePooling.Instance;
        _EventHandler = EventHandler.Instance;

        _EventHandler.OnMessageReceived += Show_Message;
        _EventHandler.OnFirstMessage += ShowFirst_Message;
        Exit_btn.onClick.AddListener(ExitGame);
    }

    public void Show_Message(string sender, string msg)
    {
        var sender_name = sender;

        foreach (var item in _Client._AllUser)
        {
            if (item.Key == sender)
            {
                sender_name = item.Value.name;
                break;
            }
          
        }
        UnityMainThreadDispatcher.Instance().Enqueue(_MessagePooling.Enqueue_OutPoolmsg(sender_name, msg));
    }

    public void ShowFirst_Message(string[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            var result = JsonConvert.DeserializeObject<Data>(arr[i]);
            Show_Message(result.Value[0], result.Value[1]);
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    public IEnumerator Show_Alluser()
    {
        foreach (var item in MessagePooling.Instance.poolDictionary["user"])
        {
            item.GetComponent<User_Model>().myText.text = string.Empty;
            item.SetActive(false);
        }


        foreach (var item in _Client._AllUser)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(_MessagePooling.SpawnFormPool("user", item.Value.name, string.Empty));
        }
        yield return null;
    }

    public IEnumerator OnConnectServerUI()
    {
        _MyName.gameObject.SetActive(false);
        _Connectbtn.gameObject.SetActive(false);
        _MyInput.gameObject.SetActive(true);
        _Sendbtn.gameObject.SetActive(true);

        yield return null;  
    }
    public IEnumerator OnDisconnectServerUI()
    {
        _MyName.gameObject.SetActive(true);
        _Connectbtn.gameObject.SetActive(true);
        _MyInput.gameObject.SetActive(false);
        _Sendbtn.gameObject.SetActive(false);

        yield return null;
    }

}
