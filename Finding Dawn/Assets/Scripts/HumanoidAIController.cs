using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAIController : MonoBehaviour {

	public Transform mainCharacter;
	private Renderer myRenderer;
	private CharacterController myCharacterController;
	private bool patrol = true;
	public GameObject[] waypoints;
	int currentWaypoint;
	public float accuracyWaypoint = 5.0f;
	public float patrolSpeed = 3.0f;
	public float alertSpeed = 10.0f;
	public float patrolRotationSpeed = 0.01f;
	public float alertRotationSpeed = 0.1f;


	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<Renderer> ();
		myCharacterController = GetComponent<CharacterController> ();

		waypoints = GameObject.FindGameObjectsWithTag ("WaypointHumanoid");
		currentWaypoint = Random.Range (0, waypoints.Length);
		GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera")[0];
		mainCharacter = mainCamera.transform;
	}

	void update() {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 direction = mainCharacter.position - this.transform.position;
		float angle = Vector3.Angle (direction, this.transform.forward);
		direction.y = 0;

		if (patrol && waypoints.Length > 0) {
			//patrol
			if (Vector3.Distance (waypoints [currentWaypoint].transform.position, transform.position) < accuracyWaypoint) {
				//select random waypoint to patrol towards
				currentWaypoint = Random.Range(0, waypoints.Length);
			}

			//rotate towards current waypoint
			direction = waypoints[currentWaypoint].transform.position - this.transform.position;
			direction.y = 0;
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), patrolRotationSpeed);
			myCharacterController.Move(this.transform.forward * Time.deltaTime * patrolSpeed);
		}

		if (Vector3.Distance (mainCharacter.position, this.transform.position) < 15 && (angle < 30 || !patrol)) {
			//AI alerted, pursue main character
			patrol = false;
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
			myRenderer.material.color = Color.red;
			myCharacterController.Move(this.transform.forward * Time.deltaTime * alertSpeed);
		} else {
			//not alert
			myRenderer.material.color = Color.blue;
			patrol = true;
		}
	}

}
