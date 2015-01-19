using UnityEngine;
using System.Collections;
using System.Linq;

public class HeroControl : MonoBehaviour {

    public float speed = 1;
    public int heroId;
    [HideInInspector]
    public string heroName;
    private RealtimeManager realtimeManager;
    public GameManager gameManager;
    private string _oldmessage = "";
    private float timePassedToShoot = 0;
    private float timePassedToLive = 0;
	
	void Start () {
        if (heroId == 0)
            heroId = Random.Range(0,10000);

        Random.seed = heroId;

        this.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.5f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        var position = new Vector3(Random.Range(-21, 21), Random.Range(-16, 16), 0);
        this.transform.position = position;

        realtimeManager = FindObjectOfType<RealtimeManager>();
        gameManager = FindObjectOfType<GameManager>();
	}
	
	void Update () {
        var xDirection = Input.GetAxis("Horizontal");
        var yDirection = Input.GetAxis("Vertical");

        var angle = Mathf.Atan2(xDirection, yDirection) * Mathf.Rad2Deg;

        if (xDirection != 0 || yDirection != 0)
        {
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        }

        this.rigidbody2D.velocity = new Vector2(xDirection * speed, yDirection * speed);
        var message = string.Format("{0}|{1}|p|{2}|{3}|{4}|{5}", heroId, heroName, this.transform.position.x, this.transform.position.y, xDirection, yDirection);
        if (message != _oldmessage || timePassedToLive > 10)
        {
            realtimeManager.RealtimeSendMessage(message);
            _oldmessage = message;
            timePassedToLive = 0;
        }

        timePassedToLive += Time.deltaTime;
        timePassedToShoot += Time.deltaTime;

        if (timePassedToShoot > 2)
        {
            if (gameManager.enemies.Count > 0)
            {
                GameObject nearestEnemy = null;
                float nearestDistance = -1f;

                foreach(var enemy in gameManager.enemies)
                {
                    var newDistance = Vector3.Distance(enemy.Value.transform.position, this.transform.position);
                    if(nearestDistance < 0 || newDistance < nearestDistance)
                    {
                        nearestDistance = newDistance;
                        nearestEnemy = enemy.Value;
                    }
                }

                var directionVector = nearestEnemy.transform.position - this.transform.position;
                directionVector.Normalize();
                var bullet = (GameObject)GameObject.Instantiate(gameManager.bulletPrefab, this.transform.position, this.transform.rotation);
                bullet.rigidbody2D.velocity = directionVector;
                bullet.GetComponent<BulletScript>().parent = this.gameObject;
                bullet.GetComponent<BulletScript>().parentName = heroName;
                bullet.GetComponentInChildren<Light>().color = Color.red;

                realtimeManager.RealtimeSendMessage(string.Format("{0}|{1}|s|{2}|{3}|{4}|{5}", heroId, heroName, this.transform.position.x, this.transform.position.y, directionVector.x, directionVector.y));
                timePassedToShoot = 0;
            }
        }
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        var bulletScript = other.gameObject.GetComponent<BulletScript>();

        if (bulletScript != null && bulletScript.parent != this.gameObject)
        {
            var position = new Vector3(Random.Range(-21, 21), Random.Range(-16, 16), 0);
            this.transform.position = position;
            var message = string.Format("{0}|{1}|p|{2}|{3}|{4}|{5}", heroId, heroName, this.transform.position.x, this.transform.position.y, 0, 0);
            realtimeManager.RealtimeSendMessage(message);
            if (bulletScript.parentName != "noname")
            {
                realtimeManager.UpdateScoreKills(bulletScript.parentName);
            }
        }
    }

    void OnApplicationQuit()
    {
        realtimeManager.RealtimeSendMessage(string.Format("{0}|{1}|q", heroId, heroName));
    }
}