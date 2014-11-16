using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	
	public struct Parabola {
		public float h;
		public float deltaT;
	};

	public enum STATE
	{
		RUNNING,
		AERIAL,
		STAND_BY
	};

	public STATE state = STATE.AERIAL;
	public float height;
	public float jumpPower;
	public float gravity;
	public bool useGravity;

	private void Jump(){
		rigidbody.velocity += jumpPower*transform.up;
		state = STATE.AERIAL;
	}

	private void Fall(){
		state = STATE.AERIAL;
	}

	private void RunningUpdate(){
		if (Input.GetKey(KeyCode.Space)){
			Debug.Log("I jump");
			Jump ();
		}

		if (!Physics.Raycast (this.transform.position, -transform.up, height/2 + 1E-1F)) {
			Fall ();
		}
	}

	private void AerialUpdate(){
		if (Physics.Raycast (transform.position, -transform.up, height/2)) {
			state = STATE.RUNNING;
			rigidbody.velocity = Vector3.zero;
			transform.position = new Vector3(0, transform.position.y, 0);
		}
	}

	private void StandByUpdate(){
		if (Input.GetKeyDown(KeyCode.Space)){
			useGravity = true;
			Fall();
		}
	}

	private void ChangingUpdate(){

	}

	// Use this for initialization
	void Start () {
		state = STATE.STAND_BY;
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		if (useGravity) {
			rigidbody.AddForce(rigidbody.mass * gravity * Vector3.down);		
		}

		switch (state) {
		case STATE.STAND_BY:
			StandByUpdate();
			break;
		case STATE.RUNNING:
			RunningUpdate();
			break;
			//case STATE.CHANGING:
			//	ChangingUpdate();
			//	break;
		case STATE.AERIAL:
			AerialUpdate();
			break;
		}
	}
}
