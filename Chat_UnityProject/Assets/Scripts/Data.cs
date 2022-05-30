using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Data
{
    public string Type { get; set; }
    public string[] Value { get; set; }
}

[System.Serializable]
public class DataList
{
    public List<DataList> Data_s { get; set; }
    //public User[] users;
}
