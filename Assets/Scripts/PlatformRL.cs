using UnityEngine;
using System.Collections;

public class PlatformRL : MonoBehaviour {

	public bool sendRight;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnCollisionStay(Collision collision){
		GameObject collider = collision.collider.gameObject;
		if (collider.tag == "Hero") {
			RaycastHit hit;
			if (Physics.Raycast(transform.position,transform.up,out hit)){
				GameObject col = hit.collider.gameObject;
				if (col.tag == "Hero"){
					LevelGenerator.GetInstance().StartTransition(sendRight);
				}
			}
		}
	}
}

