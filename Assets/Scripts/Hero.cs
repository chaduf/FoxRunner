using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	
	public struct Parabola {
		public float h;
		public float deltaT;
	};

	public enum STATE
	{
		DEAD,
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
	public float maxDepth;

	public int coin;
	public int maxLife;
	public int life;

	// Mode Gosht

	// Vitesse des animations
	public float animRunSpeed;
	public float animJumpSpeed;
	
	private int platformContacts;

	private void Jump(float power){
		transform.Translate (new Vector3 (0, 1E-1F, 0));
		rigidbody.velocity += power*transform.up;
		state = STATE.AERIAL;

	}

	private void RunningUpdate(){
		//animation.Play ("Run");
		if (Input.GetKey(KeyCode.Space)){
			Jump (jumpPower);

		}
	}

	private void AerialUpdate(){
		//this.animation.Play ("Jump");
		if (Physics.Raycast (transform.position, -transform.up, height/2)) {
			Debug.Log("land");
			state = STATE.RUNNING;
			ParticleSystem landFx = (ParticleSystem)Instantiate(landFxPrefab, transform.position - (height - 1E-1F) * transform.up, Quaternion.identity);
			Destroy (landFx);
			rigidbody.velocity = Vector3.zero;

		}

		if (transform.position.y < -maxDepth){
			Debug.Log ("died");
			Die ();
		}
	}

	private void StandByUpdate(){
		this.transform.position = Vector3.zero;
	}

	private void ChangingUpdate(){

	}

	public void ChangeRoad(){
		if (state == STATE.RUNNING){
			Jump (changeJumpPower);
		}
	}

	public void StartLevel(){
		useGravity = true;
		state = STATE.AERIAL;
	}

	public void Wait(){
		state = STATE.STAND_BY;
		transform.position = Vector3.zero;
		useGravity = false;
	}

	public void Die(){
		state = STATE.DEAD;
		coin = 0;
		if (life > 0) {
			life--;
		}
	} 

	// Use this for initialization
	public void Start () {
		gameObject.tag = "Hero";
		state = STATE.STAND_BY;
		life = maxLife;

		animRunSpeed = 13.0f;
		animJumpSpeed = 1.0f;
		//this.animation["Run"].speed = animRunSpeed;
		//this.animation["Jump"].speed = animJumpSpeed;

	}

	// Update is called once per frame
	void Update () {
//		Debug.Log (platformContacts);
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
		case STATE.AERIAL:
			AerialUpdate();
			break;
		}
	}

	void OnCollisionEnter(Collision collision){
		GameObject collider = collision.gameObject;
		if (collider.tag == "Platform"){
			platformContacts++;
			if (state == STATE.AERIAL){
				state = STATE.RUNNING;
			}
		}
	}

	void OnCollisionExit(Collision collision) {
		GameObject collider = collision.collider.gameObject;
		if (collider.tag == "Platform"){
			platformContacts--;
			if (platformContacts == 0){
				state = STATE.AERIAL;
			}
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "PlatformEdge"){
			Die ();
		}
	}
}
