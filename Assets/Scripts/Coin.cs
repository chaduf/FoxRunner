using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	// Use this for initialization
	public ParticleSystem fxPrebab;
	public float rotationSpeed;
	public int value;


	void Start ()
	{
		rotationSpeed = 300.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate (Vector3.forward, Time.deltaTime * rotationSpeed);
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Hero"){
			Hero HeroScript = (Hero)collider.gameObject.GetComponent<Hero>();
			HeroScript.coin += value;

			ParticleSystem fx = (ParticleSystem)Instantiate(fxPrebab, transform.position, Quaternion.identity);
			Destroy(fx, 0.5F);
			Destroy (gameObject);
		}
	}

}
