using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameObject parent;
    public string parentName;
    public float maxX, minX, maxY, minY;
	
	void Update ()
    {
        var oldPosition = this.transform.position;
        if (oldPosition.x > maxX || oldPosition.x < minX || oldPosition.y > maxY || oldPosition.y < minY)
        {
            Destroy(this.gameObject);
        }
	}
}