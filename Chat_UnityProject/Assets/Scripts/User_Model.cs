using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User_Model : MonoBehaviour
{
    public Text myText;
  
    public IEnumerator SetUser(string name)
    {
        var text = gameObject.GetComponentInChildren<Text>();

        text.text = name;

        yield return null;
    }
}
