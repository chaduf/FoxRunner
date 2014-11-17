using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	// Use this for initialization
	public GameObject scoreCounter;
	public ParticleSystem fxPrebab;

	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnTriggerEnter(Collider collider){
		Score scoreScript = scoreCounter.GetComponent<Score>();

		if (collider.gameObject == scoreScript.hero){
			scoreScript.IncrementScore(1);

			ParticleSystem fx = (ParticleSystem)Instantiate(fxPrebab, transform.position, Quaternion.identity);
			Destroy(fx, 0.5F);
			Destroy (gameObject);
		}
	}

}
