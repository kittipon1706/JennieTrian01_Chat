using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User_Model : MonoBehaviour
{

    public void SetUser(string name)
    {
        var text = gameObject.GetComponentInChildren<Text>();

        text.text = name;
    }
}
