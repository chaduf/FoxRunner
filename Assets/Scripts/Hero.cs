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

	public ParticleSystem landFxPrefab;
	public STATE state = STATE.AERIAL;
	public float height;
	public float jumpPower;
	public float changeJumpPower;
	public float gravity;
	public bool useGravity;

	private void Jump(float power){
		Debug.Log("I jump");
		transform.Translate (new Vector3 (0, 1E-1F, 0));
		rigidbody.velocity += power*transform.up;
		state = STATE.AERIAL;
	} 

	private void Fall(){
		state = STATE.AERIAL;
	}

	private void RunningUpdate(){
		if (Input.GetKey(KeyCode.Space)){
			Jump (jumpPower);
		}

//		if (!Physics.Raycast (transform.position, -transform.up, height/2 + 10.0F)) {
		if (!rigidbody.detectCollisions) {
			Debug.Log("I fall");
			Fall ();
		}
	}

	private void AerialUpdate(){
		if (Physics.Raycast (transform.position, -transform.up, height/2)) {
			Debug.Log("I land");
			state = STATE.RUNNING;
			ParticleSystem landFx = (ParticleSystem)Instantiate(landFxPrefab, transform.position - (height - 1E-1F) * transform.up, Quaternion.identity);
			Destroy (landFx, 0.5F);
			rigidbody.velocity = Vector3.zero;
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

	public void ChangeRoad(){
		if (state == STATE.RUNNING){
			Debug.Log("I change");
			Jump (changeJumpPower);
		}
	}

	// Use this for initialization
	void Start () {
		state = STATE.STAND_BY;
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		transform.position.Set(0, transform.position.y, 0);
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
