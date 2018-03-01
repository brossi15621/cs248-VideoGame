using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour {
	private GameManagerScript manager;
	public GameObject snake;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && manager.numSnakesChasing > 0) {//Player enters and is being chased
			//Send out a snake and increase the count on how many snakes are chasing you.
			GameObject currSnake = Instantiate(snake, gameObject.transform.position, Quaternion.identity, gameObject.transform);
			manager.addSnake (currSnake);
		} 
	}
}
