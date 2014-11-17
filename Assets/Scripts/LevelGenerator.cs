using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {

	public enum STATE
	{
		STATIC,
		TRANSITION
	};


	public GameObject[] levelBlockPrefabs;
	public GameObject startLevelBlock;
	public GameObject heroPrefab;
	public GameObject hero;

	public STATE state = STATE.STATIC;
	public LevelBlock.ORIENTATION startLevelBlockOrientation;

	public GameObject[] possibleOrientation;
	public int currentOrientation;
	public float transitionSpeed;

	public float levelBlockStep;
	public Vector3 startPosition;
	public float levelBlockSpeed;

	private GameObject lastBlock = null;

	//Creates level block
	private void CreateBlock(GameObject blockPrefab, LevelBlock.ORIENTATION orientation){
		GameObject nextBlock = (GameObject)Instantiate (blockPrefab, startPosition, Quaternion.identity);
		LevelBlock nextBlockScript = nextBlock.GetComponent<LevelBlock> ();
		nextBlock.transform.parent = this.transform;
		nextBlockScript.orientation = orientation;

		if (lastBlock != null) {
			LevelBlock lastBlockScript = lastBlock.GetComponent<LevelBlock> ();
			float correction = (nextBlockScript.size+lastBlockScript.size)/2 + lastBlock.transform.position.z - nextBlock.transform.transform.position.z;
			nextBlock.transform.Translate(0, 0, correction);
		}

		lastBlock = nextBlock;

	}

	private void TransitionRequest (){

	}

	private void SpawnHero(){

	}

	// Use this for initialization

	private void StaticUpdate() // Charles Henry le maniaque
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow)){
			hero.GetComponent<Hero>().ChangeRoad();
			currentOrientation = (++currentOrientation)%6;
			state = STATE.TRANSITION;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)){
			hero.GetComponent<Hero>().ChangeRoad();
			currentOrientation = (--currentOrientation)%6;
			if (currentOrientation < 0)
				currentOrientation += 6;
			state = STATE.TRANSITION;
		}
	}

	private void TransitionUpdate()
	{
		float difference;

		difference = Vector3.Cross (transform.up, possibleOrientation [currentOrientation].transform.up).magnitude;
		transform.rotation = Quaternion.Slerp(this.transform.rotation, 
		                                      possibleOrientation[currentOrientation].transform.rotation, 
		                                      Time.deltaTime * transitionSpeed/difference);

		difference = Vector3.Cross (transform.up, possibleOrientation [currentOrientation].transform.up).magnitude;
		if (difference < Vector3.kEpsilon){
			state = STATE.STATIC;
		}
	}

	void Start ()
	{
		CreateBlock (startLevelBlock, startLevelBlockOrientation);
		LevelBlock.speed = levelBlockSpeed;
	}
	
	// Update is called once per frame
	void Update () {

		LevelBlock.speed = levelBlockSpeed;
		
		if (startPosition.z - lastBlock.transform.position.z > levelBlockStep) {
			int index = Random.Range (0, levelBlockPrefabs.Length);
			LevelBlock.ORIENTATION orientation = (LevelBlock.ORIENTATION)Random.Range(0, 6);
			CreateBlock(levelBlockPrefabs[index], orientation);	
		}

		switch (state) 
		{
		case STATE.STATIC:
			StaticUpdate();
			break;

		case STATE.TRANSITION:
			TransitionUpdate();
			break;
		}
	}
}
