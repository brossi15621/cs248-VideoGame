using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantAIController : MonoBehaviour
{

	private Transform mainCharacter;
	private CharacterController myCharacterController;
	private bool patrol = true;
	private GameObject[] waypoints;
	public GameObject waypointParent;
	int currentWaypoint;
	public float accuracyWaypoint = 20.0f;
	public float patrolSpeed = 3.0f;
	public float alertSpeed = 7.0f;
	public float patrolRotationSpeed = 0.01f;
	public float alertRotationSpeed = 0.1f;
	public float findAngle = 50f;
	public float findDistance = 50f;
	public float killDistance = 5f;
	private Animator animController;
	private GameManagerScript manager;

	//private float gravity = 0f;

	// Use this for initialization
	void Start ()
	{
		myCharacterController = GetComponent<CharacterController> ();
		animController = gameObject.GetComponent<Animator> ();
		manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();
		GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera") [0];
		mainCharacter = mainCamera.transform;

		//Getting waypoint
		waypoints = new GameObject[waypointParent.transform.childCount];
		for (int i = 0; i < waypoints.Length; i++) {
			waypoints [i] = waypointParent.transform.GetChild (i).gameObject;
		}
		currentWaypoint = Random.Range (0, waypoints.Length);
	}


	void Update ()
	{
		//Setting character death bool to true
		float distance = Vector3.Distance (mainCharacter.position, this.transform.position);
		if (distance <= killDistance) {
			manager.dead = true;
		}
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!animController.GetBool ("inSafeZone")) { //If it's not in a safe zone
			Vector3 direction = mainCharacter.position - this.transform.position;
			float angle = Vector3.Angle (direction, this.transform.forward);
			//gravity -= 9.81f * Time.deltaTime;
			direction.y = 0f; //gravity;


			if (patrol && waypoints.Length > 0) {
				//patrol
				if (Vector3.Distance (waypoints [currentWaypoint].transform.position, transform.position) < accuracyWaypoint) {
					//select random waypoint to patrol towards
					currentWaypoint = Random.Range (0, waypoints.Length);
				}

				//rotate towards current waypoint
				direction = waypoints [currentWaypoint].transform.position - this.transform.position;
				direction.y = 0f; // gravity;

				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), patrolRotationSpeed);
				myCharacterController.Move (this.transform.forward * Time.deltaTime * patrolSpeed);
			}
			if (lineOfSight (angle)) {
				//AI alerted, pursue main character
				patrol = false;
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
				myCharacterController.Move (this.transform.forward * Time.deltaTime * alertSpeed);
			} else {
				//not alert
				patrol = true;
			}		
		}
	}


	private bool lineOfSight (float angle)
	{
		RaycastHit hit;
		//Check Distance
		if (Vector3.Distance (mainCharacter.position, this.transform.position) < findDistance
			//Check angle
		    && (!patrol || angle < findAngle)) {
			//In finding area now check for barriers
			if (!Physics.Linecast (transform.position, mainCharacter.position, out hit) || hit.transform.tag == "Player") {
				//If there is a direct line between player and giant. Meaning terrain is not in way.
				animController.SetBool ("isRunning", true);
				return true;
			}
		}
		animController.SetBool ("isRunning", false);
		return false;
	}

	public void inSafeZone (bool isCandle)
	{
		animController.SetBool ("inSafeZone", true);
		if (isCandle) {
			StartCoroutine (waitOutCandle ());
		}
	}

	public void outOfSafeZone ()
	{
		animController.SetBool ("inSafeZone", false);
	}

	IEnumerator waitOutCandle ()
	{
		yield return new WaitForSeconds (7);
		outOfSafeZone ();
	}

}
