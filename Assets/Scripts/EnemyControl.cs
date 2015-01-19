using UnityEngine;
using System.Collections;

public class EnemyControl : MonoBehaviour {

    public Vector3 newPosition;

	void Update () {
        var oldPosition = this.transform.position;
        var velocity = new Vector2((newPosition.x - oldPosition.x), (newPosition.y - oldPosition.y));
        this.rigidbody2D.velocity = velocity;
	}
}