using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Queue<KeyValuePair<string,string>> OutPoolmsg = new Queue<KeyValuePair<string, string>>();
    public Queue<string> OutPooluser = new Queue<string>();
    
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        UnityMainThreadDispatcher.Instance().Enqueue(SetupPool());
    }
    private void Update()
    {
        if (OutPoolmsg.Count > 0 )
        {
            UnityMainThreadDispatcher.Instance().Enqueue(SpawnFormPool("msg", OutPoolmsg.Peek().Key, OutPoolmsg.Peek().Value));
            UnityMainThreadDispatcher.Instance().Enqueue(Dequeue_OutPoolmsg());
        }

        if (OutPooluser.Count > 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(SpawnFormPool("user", OutPooluser.Peek(), OutPooluser.Peek()));
            UnityMainThreadDispatcher.Instance().Enqueue(Dequeue_OutPooluser());
        }
    }
    IEnumerator SpawnFormPool(string tag , string sender , string message)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag" + tag + "doesn't work");
            yield return null;
        }
        else Debug.Log("Can use pool");

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetAsLastSibling();

        Text_Model text = objectToSpawn.GetComponent<Text_Model>();
        User_Model user = objectToSpawn.GetComponent<User_Model>();

        if (user != null)
        {
            user.SetUser(sender);
        }

        if (text != null)
        {
            text.SetText(sender,message);
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        

        yield return objectToSpawn;
    }



    IEnumerator SetupPool()
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

    public IEnumerator Dequeue_OutPoolmsg()
    {
        OutPoolmsg.Dequeue();
        yield return null;
    } 
    public IEnumerator Dequeue_OutPooluser()
    {
        OutPooluser.Dequeue();
        yield return null;
    }

    public IEnumerator Enqueue_OutPoolmsg(string sender , string msg)
    {
        OutPoolmsg.Enqueue(new KeyValuePair<string, string>(sender, msg));
        yield return null;
    }
    public IEnumerator Enqueue_OutPooluser(string name)
    {
        OutPooluser.Enqueue(name);
        yield return null;
    }
}
