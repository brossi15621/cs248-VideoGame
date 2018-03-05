using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeWarningZone : MonoBehaviour {
	private GameObject snake;
	private Light snakeLight;
	private SphereCollider dangerZone;
	private SphereCollider foundZone;
	private SnakeAIController snakeScript;
	private bool inDangerZone;

	public const float addedRadius = 4.0f;
	// Use this for initialization
	void Start () {
		snake = this.transform.parent.gameObject;
		snakeLight = snake.GetComponentInChildren<Light> ();
		dangerZone = gameObject.GetComponent<SphereCollider> ();
		foundZone = snake.GetComponent<SphereCollider> ();
		snakeScript = snake.GetComponent<SnakeAIController> ();
	}
	
	// Update is called once per frame
	void Update () {
		dangerZone.radius = foundZone.radius + addedRadius;
		if (!snakeScript.getPatrolling()) {
			snakeLight.color = Color.red;
		} else if (inDangerZone) {
			snakeLight.color = Color.yellow;
		} else {
			snakeLight.color = Color.green;
		}
	}

	void OnTriggerStay(Collider other) {
		if (snakeScript.getPatrolling() && other.tag == "Player") {
			inDangerZone = true;
		}
	}

	void OnTriggerExit(Collider other){
		if (snakeScript.getPatrolling() && other.tag == "Player") {
			inDangerZone = false;
		}
	}
}
