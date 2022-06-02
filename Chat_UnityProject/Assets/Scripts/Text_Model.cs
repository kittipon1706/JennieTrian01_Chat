using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Model : MonoBehaviour
{

    [SerializeField] private Text _MyText;
    [SerializeField] private Text _ChildText;
    [SerializeField] private Image _Bg;


    public IEnumerator SetText(string Myname,string sender_name, string message)
    {
        RectTransform rt = _ChildText.gameObject.GetComponent<RectTransform>();
        if (Myname == sender_name)
        {
            _MyText.text = string.Empty;
            _MyText.alignment = TextAnchor.MiddleRight;
            _MyText.text = message + "   ";

            _ChildText.text = "Me : ";
            rt.pivot = new Vector2(1, 0);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, rt.rect.width);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rt.rect.height);
            rt.localPosition = new Vector3(rt.localPosition.x, 70f, rt.localPosition.z);
            _ChildText.alignment = TextAnchor.MiddleRight;

        }
        else
        {
            if (sender_name == "Server")
            {
                _ChildText.color = Color.yellow;
                _Bg.color = Color.yellow;
                message = "Welcome to server : " + Client.Instance.Name;
            }

            _MyText.text = string.Empty;
            _MyText.alignment = TextAnchor.MiddleLeft;
            _MyText.text = "   " + message;
            
            _ChildText.text = sender_name + " : ";
            rt.pivot = new Vector2(0, 0);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rt.rect.width);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rt.rect.height);
            rt.localPosition = new Vector3(rt.localPosition.x, 70f, rt.localPosition.z);
            _ChildText.alignment = TextAnchor.MiddleLeft;
        }
        yield return null;
    }

    
}
