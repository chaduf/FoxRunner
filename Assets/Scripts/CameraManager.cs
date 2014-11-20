using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public enum STATE{
		LOOK_HERO,
		LOOK_FORWARD
	}
	
	public STATE state = STATE.LOOK_FORWARD;
	public GameObject hero;
	public float rotationSpeed;

	private GameObject movSmoother;

	private void lookForwardUpdate(){
		movSmoother.transform.position = transform.position;
		movSmoother.transform.rotation = Quaternion.identity;

		if (hero) {
			state = STATE.LOOK_HERO;
			return;
		}

		transform.rotation = Quaternion.Slerp (transform.rotation, 
		                                       movSmoother.transform.rotation, 
		                                       rotationSpeed * Time.deltaTime);
	}

	private void LookHeroUpdate(){
		movSmoother.transform.position = transform.position;
		movSmoother.transform.LookAt(hero.transform.position);

		if (!hero) {
			state = STATE.LOOK_FORWARD;
			return;
		}
		
		transform.rotation = Quaternion.Slerp (transform.rotation, 
		                                       movSmoother.transform.rotation, 
		                                       rotationSpeed * Time.deltaTime);
	}

	// Use this for initialization
	void Start () {
		movSmoother = new GameObject();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case STATE.LOOK_FORWARD:
			lookForwardUpdate();
			break;
		case STATE.LOOK_HERO:
			LookHeroUpdate();
			break;
		}
	}
}
