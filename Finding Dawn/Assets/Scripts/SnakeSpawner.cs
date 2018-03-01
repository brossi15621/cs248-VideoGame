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
		if (other.tag == "Player" && manager.numSnakesFound > 0) {
			GameObject currSnake = Instantiate(snake, gameObject.transform.position, Quaternion.identity, gameObject.transform);
			currSnake.GetComponent<SnakeAIController> ().patrol = false;
		} 
	}
}
