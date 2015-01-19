using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject hero;
    public Dictionary<int,GameObject> enemies;
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public RealtimeManager realtimeManager;

    [HideInInspector]
    public Queue<string> messages;

    private int heroId;
    private string heroName;

	void Start ()
    {
        if (!File.Exists("config.txt")) 
            File.WriteAllText("config.txt", "noname", Encoding.UTF8);

        heroName = File.ReadAllText("config.txt");

        messages = new Queue<string>();
        enemies = new Dictionary<int, GameObject>();
        realtimeManager = GetComponent<RealtimeManager>();
        heroId = hero.GetComponent<HeroControl>().heroId;

        hero.GetComponentInChildren<Text>().text = heroName;
        hero.GetComponent<HeroControl>().heroName = heroName;

        realtimeManager.RealtimeStart();
	}
	
	void Update ()
    {
        if (messages != null)
        {
            lock (messages)
            {
                while(messages.Count > 0)
                {
                    MessageHandle(messages.Dequeue());
                }
            }
        }
	}

    void MessageHandle(string message)
    {
        var parts = message.Split('|');
        var player = int.Parse(parts[0]);
        var playerName = parts[1];
        var command = parts[2];

        if (player != heroId)
        { 
            if (!enemies.ContainsKey(player))
            {
                var enemy = (GameObject)GameObject.Instantiate(playerPrefab);
                Random.seed = player;
                enemy.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.5f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                enemy.GetComponentInChildren<Text>().text = playerName;
                enemies.Add(player, enemy);
            }

            if(command == "p")
            {
                var positionX = float.Parse(parts[3]);
                var positionY = float.Parse(parts[4]);
                var directionX = float.Parse(parts[5]);
                var directionY = float.Parse(parts[6]);
                 
                var newPosition = new Vector3(positionX, positionY, 0);
                
                enemies[player].GetComponent<EnemyControl>().newPosition = newPosition;
                var angle = Mathf.Atan2(directionX, directionY) * Mathf.Rad2Deg;
                if (directionX != 0 || directionY != 0)
                    enemies[player].transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
            }
            else if(command == "s")
            {
                var positionX = float.Parse(parts[3]);
                var positionY = float.Parse(parts[4]);
                var directionX = float.Parse(parts[5]);
                var directionY = float.Parse(parts[6]);

                var bullet = (GameObject)GameObject.Instantiate(bulletPrefab, new Vector3(positionX, positionY, 0f), enemies[player].transform.rotation);
                bullet.rigidbody2D.velocity = new Vector2(directionX, directionY);
                bullet.GetComponent<BulletScript>().parent = enemies[player];
                bullet.GetComponent<BulletScript>().parentName = playerName;
            }
            else if(command == "q")
            {
                Destroy(enemies[player]);
                enemies.Remove(player);
            }
        }
    }
}