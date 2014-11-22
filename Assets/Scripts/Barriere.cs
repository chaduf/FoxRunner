using UnityEngine;
using System.Collections;

public class Barriere : MonoBehaviour {

	// Use this for initialization


	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider col)
	{

		if (col.gameObject.tag == "Hero") 
		{
			Hero heroScript = col.gameObject.GetComponent<Hero>();
			Debug.Log("collision");
			heroScript.Die();
		}
	}
}
