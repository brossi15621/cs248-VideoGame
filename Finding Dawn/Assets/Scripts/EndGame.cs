using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour {

	private bool endGame = false;
	private bool playedAlready = false;
	public Material daytime; 

	// Use this for initialization
	void Start () {
		
	}

	void Update(){
		if (endGame) {
			RenderSettings.ambientSkyColor = Color.Lerp (RenderSettings.ambientSkyColor, Color.white, .03f);
		}
	}
	


	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && !playedAlready) {
			playedAlready = true;
			endGame = true;
			RenderSettings.skybox = daytime;
			killAllEnemies ();
		}
	}

	private void killAllEnemies(){
		GameObject enemies = GameObject.Find("Enemies");
		for (int i = 0; i < enemies.transform.childCount; i++) {
			Transform child = enemies.transform.GetChild (i);
			killEnemy (child);
		}
	}

	private void killEnemy(Transform child){
		if (child.tag == "Snake") {
			child.GetComponentInChildren<Light> ().enabled = false;
			SnakeAIController snakeScript = child.GetComponent<SnakeAIController> ();
			snakeScript.returnToLife = false;
			snakeScript.destroySnake ();
		} else if (child.tag == "Humanoid") {
			child.GetComponent<HumanoidAIController> ().enterSafeZone (false);
		} else if (child.tag == "Giant") {
			child.GetComponent<GiantAIController> ().destroyGiant ();
		}
	}
}
