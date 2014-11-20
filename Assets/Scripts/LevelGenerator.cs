﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

	public enum STATE
	{
		DISABLED,
		TRANSITION,
		SCROLLING,
		STARTING
	};
	
	public GameObject[] levelBlockPrefabs;
	private ArrayList createdBlocks;
	public GameObject startLevelBlock;
	public GameObject camera;
	
	public GameObject heroPrefab;
	public GameObject hero;
	public float startingDuration;
	private float startingTime;

	public STATE state = STATE.SCROLLING;
	public LevelBlock.ORIENTATION startLevelBlockOrientation;

	public GameObject[] possibleOrientation;
	public int currentOrientation;
	public float transitionSpeed;
	public float leveBlockStep;
	
	public float startPosition;
	public float endPosition;
	public float scrollingSpeed;

	public Texture2D transitionScreen;

	private GameObject lastBlock = null;
	private GameObject[] genLevelBlocks;

	private void DestroyLevel(){
		foreach (GameObject block in createdBlocks) {
			Destroy(block);		
		}
		createdBlocks.Clear ();
	}

	private void MoveBlocks (){
		ArrayList blocksToRemoveIndex = new ArrayList();

		foreach (GameObject block in createdBlocks) {
			// fill the list of Level block to remove (with indexes)
			if (block.transform.position.z < endPosition){
				blocksToRemoveIndex.Add (createdBlocks.IndexOf(block));
				continue;
			}
			
			// Translates Level blocks
			block.transform.Translate (new Vector3(0,0,-scrollingSpeed*Time.deltaTime));
		}
		
		// Removes level blocks to remove
		foreach (int i in blocksToRemoveIndex) {
			Destroy((GameObject)createdBlocks[i]);
			createdBlocks.RemoveAt(i);
		}
	}

	private void Scroll(){
		if (startPosition - lastBlock.transform.position.z > leveBlockStep) {
			int index = Random.Range (0, levelBlockPrefabs.Length);
			LevelBlock.ORIENTATION orientation = (LevelBlock.ORIENTATION)Random.Range(0, 6);
			CreateBlock(levelBlockPrefabs[index], orientation);	
		}
		MoveBlocks ();
	}

	private void Scroll(GameObject LevelBlockPrefab, LevelBlock.ORIENTATION orientation){
		if (startPosition - lastBlock.transform.position.z > leveBlockStep) {
			CreateBlock(LevelBlockPrefab, orientation);
		}
		MoveBlocks ();
	}

	private void GetTransitionRequest(){
		Hero.STATE heroState = hero.GetComponent <Hero>().state;
		if (Input.GetKey(KeyCode.LeftArrow) && heroState == Hero.STATE.RUNNING){
			hero.GetComponent<Hero>().ChangeRoad();
			currentOrientation = (++currentOrientation)%6;
			state = STATE.TRANSITION;
		}
		if (Input.GetKey(KeyCode.RightArrow) && heroState == Hero.STATE.RUNNING){
			hero.GetComponent<Hero>().ChangeRoad();
			currentOrientation = (--currentOrientation)%6;
			if (currentOrientation < 0)
				currentOrientation += 6;
			state = STATE.TRANSITION;
		}
	}

	//Creates level block
	private void CreateBlock(GameObject blockPrefab, LevelBlock.ORIENTATION orientation, float z, bool adjust){
		GameObject nextBlock = (GameObject)Instantiate (blockPrefab, z * Vector3.forward, Quaternion.identity);
		LevelBlock nextBlockScript = nextBlock.GetComponent<LevelBlock> ();

		nextBlock.transform.parent = transform;
		nextBlockScript.orientation = orientation;

		if (lastBlock != null && adjust) {
			LevelBlock lastBlockScript = lastBlock.GetComponent<LevelBlock> ();
			float correction = (nextBlockScript.size+lastBlockScript.size)/2 + lastBlock.transform.position.z - nextBlock.transform.transform.position.z;
			nextBlock.transform.Translate(0, 0, correction);
		}

		createdBlocks.Add(nextBlock);
		lastBlock = nextBlock;
	}

	private void CreateBlock(GameObject blockPrefab, LevelBlock.ORIENTATION orientation){
		CreateBlock (blockPrefab, orientation, startPosition, true);
	}

	private void RotateLevel(){
		float difference;
		
		difference = Vector3.Cross (transform.up, possibleOrientation [currentOrientation].transform.up).magnitude;
		transform.rotation = Quaternion.Slerp(this.transform.rotation, 
		                                      possibleOrientation[currentOrientation].transform.rotation, 
		                                      Time.deltaTime * transitionSpeed/difference);
		
		difference = Vector3.Cross (transform.up, possibleOrientation [currentOrientation].transform.up).magnitude;
		if (difference < Vector3.kEpsilon){
			state = STATE.SCROLLING;
		}
	}

	private void StartLevel(){
		float step = ((LevelBlock)startLevelBlock.GetComponent<LevelBlock>()).size;
		CreateBlock (startLevelBlock, 
		             startLevelBlockOrientation, 
		             startPosition, false);

		for (float z=startPosition-step; z>endPosition; z-=step) {
			CreateBlock(startLevelBlock, 0, z, false);
		}

		hero.GetComponent<Hero>().Wait();
		Debug.Log (hero.transform.position);
		state = STATE.STARTING;
		startingTime = 0.0F;
	}

	private void RestartLevel(){
		DestroyLevel ();
		StartLevel ();
	}

	private void stopLevel (){
		DestroyLevel ();
		state = STATE.DISABLED;
	}

	private void WaitStart (){
		startingTime += Time.deltaTime;

		if (startingTime > startingDuration) {
			Debug.Log("End start");
			state = STATE.SCROLLING;
			hero.GetComponent<Hero> ().startRunning();
		}
	}

	public void DisableLevel(){
		DestroyLevel ();
		state = STATE.DISABLED;
	}
	
	public void EnableLevel(){
		if (hero){
			Destroy(hero);
		}
		
		hero = (GameObject)Instantiate (heroPrefab, 
		                                Vector3.zero, 
		                                Quaternion.identity);
		
		CameraManager camManager = camera.GetComponent<CameraManager>();
		camManager.hero = hero;
		StartLevel ();
	}

	private void checkHeroState(){
		Hero heroScript = (Hero)hero.GetComponent<Hero> ();
		switch (heroScript.state){
			case Hero.STATE.DEAD:
			if (heroScript)
				RestartLevel();
			break;
		}
	}

	private void StartingUpdate(){
		Scroll ();
		WaitStart ();
	}

	// makes the level turn
	private void TransitionUpdate(){
		RotateLevel();
		Scroll ();
		checkHeroState ();
	}

	private void ScrollingUpdate(){
		Scroll ();
		GetTransitionRequest();
		checkHeroState ();
	}

	private void StartingOnGUI(){
		Debug.Log ("Waiting GUI");
		GUI.color = Color.Lerp (Color.black, 
                                new Color(0, 0, 0, 0.0F), 
                                startingTime/startingDuration);

		GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), transitionScreen);

		GUI.color = Color.white;
	}

	void Start ()
	{
		createdBlocks = new ArrayList ();
		EnableLevel ();
	}

	// Update is called once per frame
	void Update () {

		switch (state) 
		{
		case STATE.SCROLLING:
			ScrollingUpdate ();
			break;
		case STATE.TRANSITION:
			TransitionUpdate();
			break;
		case STATE.STARTING:
			StartingUpdate();
			break;
		}
	}

	void OnGUI(){
		switch (state) 
		{
		case STATE.SCROLLING:
			//ScrollingOnGUI ();
			break;
		case STATE.TRANSITION:
			//TransitionOnGUI();
			break;
		case STATE.STARTING:
			StartingOnGUI();
			break;
		}
	}
}
