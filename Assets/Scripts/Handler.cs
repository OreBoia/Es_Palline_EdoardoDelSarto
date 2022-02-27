using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class Handler : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    public Queue<GameObject> ballCounter;
    public BallEntries entries;

    private void Start()
    {
        ballCounter = new Queue<GameObject>();
        

        try
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://localhost:3000/BallGame");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                entries = JsonUtility.FromJson<BallEntries>(reader.ReadToEnd());
            }

            foreach (var entry in entries.entries)
            {
                if (!entry.death)
                {
                    GameObject newBall = Instantiate(_ballPrefab);
                    newBall.transform.position = new Vector3(entry.x, entry.y, 0);
                    Debug.Log($"Index:{entry.index}, x:{entry.x}, y:x:{entry.y}, Death:{entry.death}");
                    ballCounter.Enqueue(newBall);
                }
                else
                {
                    Debug.Log($"Ball {entry.index} is dead. Wait until 00:00 to respawn");
                }
                
            }
        }
        catch (Exception e)
        {
            Debug.Log("impossibile connettersi" + e);
        }
    }

    private void Update()
    {
        GameObject ballToDelete;
        int queueLenght = ballCounter.Count();

        if (Input.GetKeyDown(KeyCode.X))
        {
            ballToDelete = ballCounter.Dequeue();

            foreach (var entry in entries.entries)
            {
                if (!entry.death)
                {
                    if (entry.index == queueLenght - 1)
                    {
                        Destroy(ballToDelete);

                        StartCoroutine(SendDataToServer(entry.index));

                        Debug.Log($"Ball {entry.index} set to death");
                    }
                }    
            }
        }
    }

    IEnumerator SendDataToServer(int index)
    {
        Dictionary<string, string> requestParams = 
            new Dictionary<string, string>();

        requestParams.Add("index", index.ToString());
        

        UnityWebRequest request = 
            UnityWebRequest.Post("http://localhost:3000/set-death", requestParams);

        yield return request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);
    }
}
