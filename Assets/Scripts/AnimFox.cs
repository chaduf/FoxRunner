using UnityEngine;
using System.Collections;

public class AnimFox : MonoBehaviour {


	// Use this for initialization

	public enum Stats {
		JUMPING,
		RUNING
	};

	public float animRunSpeed;
	public float animJumpSpeed;

	public Stats stat;
	void Start () 
	{
		animRunSpeed = 10.0f;
		animJumpSpeed = 2.0f;
		this.animation["Run"].speed = animRunSpeed;
		this.animation["Jump"].speed = animJumpSpeed;
		stat = Stats.RUNING;

	}
	
	// Update is called once per frame
	void Update () 
	{

		switch (stat) 
		{
			case Stats.JUMPING:
				jumpUpdate ();
			break;
			case Stats.RUNING:
				runUpdate ();
			break;
		}
	}

	void jumpUpdate()
	{
		//this.animation.CrossFade ("Run",0.2f);
		this.animation.Play ("Jump");
		stat = Stats.RUNING;
				

	}

	void runUpdate()
	{


		if (animation.IsPlaying("Jump") == false)
		{
			
			this.animation.Play ("Run");

		}

		

		if (Input.GetKeyDown (KeyCode.Space)) {
			stat = Stats.JUMPING;
		}
	}
}
