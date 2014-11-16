using UnityEngine;
using System.Collections;

public class Respaw : MonoBehaviour {

	// Use this for initialization
	public GameObject player;

	void Start () 
	{
		//Collider colPlay = player.collider;

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggered(Collider colPlay)
	{
		//player.transform.position = new Vector3 (0, 0, 7.226614);
	}
}
