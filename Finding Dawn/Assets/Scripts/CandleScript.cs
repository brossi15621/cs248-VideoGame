using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleScript : MonoBehaviour {

	private Light spotLight;
	private bool deminish;

	// Use this for initialization
	void Start () {
		spotLight = gameObject.GetComponentInChildren<Light>();
		deminish = false;
		StartCoroutine (waitForBurn ());
		StartCoroutine(turnOffLight());

	}
	
	// Update is called once per frame
	void Update () {
		if (deminish) {
			spotLight.intensity = Mathf.Lerp (spotLight.intensity, 0f, .6f * Time.deltaTime);
		} else {
			spotLight.intensity = Mathf.Lerp (spotLight.intensity, 7f, .5f * Time.deltaTime);
		}
	}

	IEnumerator turnOffLight(){
		yield return new WaitForSeconds(7);
		spotLight.enabled = false;
		gameObject.GetComponent<CapsuleCollider> ().enabled = false;
	}

	IEnumerator waitForBurn(){
		yield return new WaitForSeconds(5);
		deminish = true;
	}
}
