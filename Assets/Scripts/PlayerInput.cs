using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	private CharacterController controller;
	private float speed = 5;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movement = new Vector3(Input.GetAxis ("Horizontal"), 0, 0);
		movement = movement;
		movement.Normalize();
		controller.SimpleMove(movement * speed);
	}
}
