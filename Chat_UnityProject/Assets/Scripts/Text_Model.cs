using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Model : MonoBehaviour
{
    private MessagePooling _MessagePooling;
    private Text _MyText;
    private void Start()
    {
        _MessagePooling = MessagePooling.Instance;
    }

    public void SetText(string sender, string message)
    {
        _MyText = GetComponent<Text>();

        var sender_name = string.Empty;
        foreach (var item in Client.Instance._AllUser)
        {
            if (item.Id == sender)
            {
                sender_name = item.name;
            }
        }

        if (Client.Instance.id == sender)
        {
            _MyText.text = string.Empty;
            _MyText.alignment = TextAnchor.MiddleRight;
            _MyText.text = message + " ";
        }
        else
        {
            _MyText.text = string.Empty;
            _MyText.alignment = TextAnchor.MiddleLeft;
            _MyText.text = " " + sender_name + " : " + message;
        }

    }

    
}
