using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePooling : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefabs;
        public int size;
        public GameObject Parent;
    }

    #region Singleton
    public static MessagePooling Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Queue<KeyValuePair<string,string>> OutPoolMymsg = new Queue<KeyValuePair<string, string>>();
    public Queue<KeyValuePair<string,string>> OutPoolOthermsg = new Queue<KeyValuePair<string, string>>();
    public Queue<string> OutPooluser = new Queue<string>();
    private Client _Client;

    void Start()
    {
        _Client = Client.Instance;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        UnityMainThreadDispatcher.Instance().Enqueue(SetupPool());
    }
    private void Update()
    {
        if (OutPoolMymsg.Count > 0 )
        {
            UnityMainThreadDispatcher.Instance().Enqueue(SpawnFormPool("Mymsg", OutPoolMymsg.Peek().Key, OutPoolMymsg.Peek().Value));
            OutPoolMymsg.Dequeue();
        }

        if (OutPoolOthermsg.Count > 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(SpawnFormPool("Othermsg", OutPoolOthermsg.Peek().Key, OutPoolOthermsg.Peek().Value));
            OutPoolOthermsg.Dequeue();
        }


    }

    
    public IEnumerator SpawnFormPool(string tag , string sender_name , string message)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag" + tag + "doesn't work");
            yield return null;
        }
       

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetAsLastSibling();

        Text_Model text = objectToSpawn.GetComponent<Text_Model>();
        User_Model user = objectToSpawn.GetComponent<User_Model>();

        
        if (user != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(user.SetUser(sender_name));
        }

        if (text != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(text.SetText(_Client.Name,sender_name, message));
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        yield return null;
    }

    public IEnumerator SetupPool()
    {

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefabs);
                obj.transform.SetParent(pool.Parent.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
        yield return null;
    }

    public IEnumerator Enqueue_OutPoolmsg(string sender_name , string msg)
    {
        if (sender_name == _Client.name)
        {
            OutPoolMymsg.Enqueue(new KeyValuePair<string, string>(sender_name, msg));
        }
        else OutPoolOthermsg.Enqueue(new KeyValuePair<string, string>(sender_name, msg));
        yield return null;
    }
   
}
