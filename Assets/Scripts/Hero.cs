﻿using UnityEngine;
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

	private void Jump(float power){
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
	}

	private void AerialUpdate(){
		if (Physics.Raycast (transform.position, -transform.up, height/2)) {
			state = STATE.RUNNING;
			ParticleSystem landFx = (ParticleSystem)Instantiate(landFxPrefab, transform.position - (height - 1E-1F) * transform.up, Quaternion.identity);
			Destroy (landFx, 0.5F);
			rigidbody.velocity = Vector3.zero;
		}
		Debug.Log("Dead test");
		if (transform.position.z < -maxDepth){
			Debug.Log("Dead");
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

	public void startRunning(){
		useGravity = true;
		Fall();
	}

	public void Wait(){
		state = STATE.STAND_BY;
		transform.position = Vector3.zero;
		useGravity = false;
	}

	public void Die(){
		state = STATE.DEAD;
		if (life > 0) {
			life--;
		}
	} 

	// Use this for initialization
	public void Start () {
		gameObject.tag = "Hero";
		state = STATE.STAND_BY;
		life = maxLife;
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
		case STATE.AERIAL:
			AerialUpdate();
			break;
		}
	}

	void OnCollisionEnter(Collision collision){
		GameObject collider = collision.gameObject;
		if (collider.tag == "Platform"){
			RaycastHit hit;
			if (!Physics.Raycast(transform.position, -transform.up, out hit) || !hit.collider.gameObject == collider){
				Debug.Log("I die");
				Die();
			}
			else if (state == STATE.AERIAL){
				state = STATE.RUNNING;
			}
		
		}
	}
}
