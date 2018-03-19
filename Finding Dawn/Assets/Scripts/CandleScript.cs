using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CandleScript : MonoBehaviour {

	private Light spotLight;
	private bool deminish;
	public AudioSource candleSource;
	private Transform mainCharacter;


	// Use this for initialization
	void Start () {
		GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera")[0];
		mainCharacter = mainCamera.transform;
		gameObject.GetComponent<SafeZone> ().isLantern = true;
		spotLight = gameObject.GetComponentInChildren<Light>();
		deminish = false;
		candleSource.Play ();
		StartCoroutine (waitForBurn ());
		StartCoroutine(turnOffLight());

	}
	
	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance (mainCharacter.position, this.transform.position);
		if (distance != 0) {
			//find attenuation for noise level of candle
			float noiseAttenuation = 1/distance;
			candleSource.volume = noiseAttenuation;
		}
		//Debug.Log ("Distance: " + distance);
		if (deminish) {
			spotLight.intensity = Mathf.Lerp (spotLight.intensity, 0f, .6f * Time.deltaTime);
		} else {
			spotLight.intensity = Mathf.Lerp (spotLight.intensity, 7f, .5f * Time.deltaTime);
		}
	}

	IEnumerator turnOffLight() {
		yield return new WaitForSeconds(7);
		spotLight.enabled = false;
		gameObject.GetComponent<CapsuleCollider> ().enabled = false;
		candleSource.Stop ();
	}

	IEnumerator waitForBurn() {
		yield return new WaitForSeconds(5);
		deminish = true;
	}
}
