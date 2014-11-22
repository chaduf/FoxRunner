using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {
	public GameObject startBlockPrefab;
	public GameObject[] blockPrefabs;
	public float startScrollingSpeed;
	public float minScrollingSpeed;
	public float maxScrollingSpeed;
	public float scrollingSpeedInc;
	public float scrollingSpeedDec;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public LevelGenerator.LEVEL Load(){
		LevelGenerator.LEVEL level = new LevelGenerator.LEVEL ();
		level.startBlockPrefab = startBlockPrefab;
		level.blockPrefabs = blockPrefabs;
		level.startScrollingSpeed = startScrollingSpeed;
		level.minScrollingSpeed = minScrollingSpeed;
		level.maxScrollingSpeed = maxScrollingSpeed;
		level.scrollingSpeedInc = scrollingSpeedInc;
		level.startScrollingSpeed = scrollingSpeedInc;
		level.scrollingSpeedDec = scrollingSpeedDec;
		return level;
	}
}
