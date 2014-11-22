using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public enum STATE{
		MENU,
		LEVEL_SELECT,
		GAME,
		PAUSE,
		LOAD,
		GAME_OVER,
		WIN
	}

	public Font mainFont;

	private static GameManager instance;
	//GUI management
	void OnGUI(){

	}

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameManager getInstance(){
		return instance;
	}
}
