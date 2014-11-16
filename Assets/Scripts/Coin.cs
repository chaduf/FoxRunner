using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	// Use this for initialization
	public GameObject player;

	private Collider colPlay;

	void Start () 
	{
		colPlay = player.collider;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnTriggerEnter(Collider col)
	{


		
		Score.IncrementScoreCoin();
		Debug.Log("CoinTouched");
		Destroy (this.gameObject);
		Debug.Log("Destroyed");



		
	}

}
