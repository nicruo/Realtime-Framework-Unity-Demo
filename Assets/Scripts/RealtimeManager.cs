using UnityEngine;
using System.Collections;
using Realtime.Messaging;
using System.IO;
using System.Linq;
using System.Text;
using Realtime.Storage.Models;
using Realtime.Storage.DataAccess;
using UnityEngine.UI;
using Realtime.Storage.Controllers;

public class RealtimeManager : MonoBehaviour
{

    private RealtimeMessenger Messenger;
    private StorageRepository Repository;
    private StorageController Context;

    public string Channel = "my_channel";
    private string _message;
    private GameManager gameManager;

    private bool shouldUpdateHighScore = false;


    public Text highScore;

	void Awake ()
	{
        Messenger = new RealtimeMessenger();
        Messenger.OnMessage += OnMessage;
        gameManager = GetComponent<GameManager>();
	}

    public void RealtimeStart()
    {
        this.StartCoroutine(Connect());
        this.StartCoroutine(ConnectStorage());
    }

    public void RealtimeSendMessage(object message)
    {
        _message = (string)message;
        this.StartCoroutine(Send());
    }

    void OnMessage(string channel, string message)
    {
        gameManager.messages.Enqueue(message);
    }
	
    IEnumerator Connect()
    {
        Messenger.ConnectionMetadata = "ClientMetaData";
        Messenger.AuthenticationToken = "AuthToken";

        var task = Messenger.Connect();
        
        yield return StartCoroutine(task.WaitRoutine());

        if (task.IsFaulted)
        {
            Debug.Log("Error Connect(): " + task.Exception);
        }
        else
        {
            StartCoroutine(Subscribe());
        }
    }

    IEnumerator Subscribe()
    {
        var task = Messenger.Subscribe(Channel);
        yield return StartCoroutine(task.WaitRoutine());
        if (task.IsFaulted)
        {
            Debug.Log("Error Subscribe(): " + task.Exception);
        }
        else
        {
            //StartCoroutine(SendHelloMessage());
        }
    }

    IEnumerator Send()
    {
        var task = Messenger.Send(Channel, _message);
        yield return StartCoroutine(task.WaitRoutine());
    }

    IEnumerator ConnectStorage()
    {
        Repository = new StorageRepository();

        var taskRepositoryGet = Repository.List<Score>(new ItemListRequest<Score>());
        yield return StartCoroutine(taskRepositoryGet.WaitRoutine());
        if (taskRepositoryGet.IsFaulted)
        {
            Debug.Log("Error taskRepositoryGet(): " + taskRepositoryGet.Exception);
        }
        else
        {
            var score2 = taskRepositoryGet.Result.data;
            var textHighScore = "==HIGH SCORE==";
            foreach (var scoreItem in score2.items.OrderByDescending(s => s.kills))
            {
                textHighScore += "\n" + scoreItem.kills + " - " + scoreItem.heroID;
            }
            highScore.text = textHighScore;
        }

        Context = new StorageController("authkey");
        // get score table
        var tableTask = Context.Table<Score>();
        yield return StartCoroutine(tableTask.WaitRoutine());
        tableTask.ThrowIfFaulted();
        // Use score table reference
        var table = tableTask.Result;
        table.On(StorageEventType.UPDATE | StorageEventType.PUT, response =>
        {
            shouldUpdateHighScore = true;
        });
    }

    IEnumerator UpdateHighScore()
    {
        var taskRepositoryGet = Repository.List<Score>(new ItemListRequest<Score>());
        yield return StartCoroutine(taskRepositoryGet.WaitRoutine());
        if (taskRepositoryGet.IsFaulted)
        {
            Debug.Log("Error taskRepositoryGet(): " + taskRepositoryGet.Exception);
        }
        else
        {
            var score2 = taskRepositoryGet.Result.data;
            var textHighScore = "==HIGH SCORE==";
            foreach (var scoreItem in score2.items)
            {
                textHighScore += "\n" + scoreItem.kills + " - " + scoreItem.heroID;
            }
            highScore.text = textHighScore;
        }
    }

    public void UpdateScoreKills(string heroName)
    {
        this.StartCoroutine(UpdateScore(heroName));
    }

    IEnumerator UpdateScore(string heroID)
    {
        var result2B = Repository.Get<Score>(heroID);
        yield return StartCoroutine(result2B.WaitRoutine());
        var score2 = result2B.Result.data;
        if(score2.heroID == null)
        {
            score2.heroID = heroID;
            score2.kills = 0;
        }
        score2.kills += 1;

        var result3 = Repository.Create(score2);
        yield return StartCoroutine(result3.WaitRoutine());
        result3.ThrowIfFaulted();
        if (result3.IsFaulted)
        {
            Debug.Log(result3.Exception);
        }
    }

	void Update ()
    {
        if(shouldUpdateHighScore)
        {
            this.StartCoroutine(UpdateHighScore());
            shouldUpdateHighScore = false;
        }
	}
}