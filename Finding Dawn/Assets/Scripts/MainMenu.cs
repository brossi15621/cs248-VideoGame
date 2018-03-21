using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;


public class MainMenu : MonoBehaviour {

	public Button playText;
	public Transform mainCharacter;

	// Use this for initialization
	void Start () {
		playText = playText.GetComponent<Button> ();
	}
	
	public void StartGame() {
		//load the first level of the game which should be at index 1 of build settings
		SceneManager.LoadScene(1); 
	}
}
