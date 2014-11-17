using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

	// Use this for initialization
	public GameObject hero;
	public int value;

	void Start (){
		value = 0;
	}
	
	// Update is called once per frame
	void Update (){
	
	}

	public void IncrementScore(int inc){
		value += inc;
	}
}
