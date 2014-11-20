using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public GameObject hero;
	public float rotationSpeed;

	private GameObject movSmoother;
	// Use this for initialization
	void Start () {
		movSmoother = new GameObject();
	}
	
	// Update is called once per frame
	void Update () {
		movSmoother.transform.position = transform.position;
		movSmoother.transform.LookAt(hero.transform.position);

		transform.rotation = Quaternion.Slerp (transform.rotation, 
		                                       movSmoother.transform.rotation, 
		                                       rotationSpeed * Time.deltaTime);
	}
}
