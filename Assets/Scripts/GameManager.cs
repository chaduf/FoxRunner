using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public enum STATE{
		MENU,
		LEVEL_SELECT,
		GAME,
		PAUSE,
		LOADING,
		GAME_OVER,
		WIN
	}

	private static GameManager instance;

	public STATE state = STATE.MENU;

	public Font mainFont;
	public GameObject[] levels;
	public GameObject levelGenerator;
	public Texture2D titleTexture;
	public Texture2D emptyScreenTexture;
	public Texture2D gameOverTexture;

	private GUIStyle textStyle; 

	private IEnumerator LoadLevel(int index){
		Level levelScript = levels [index].GetComponent<Level> ();
		LevelGenerator levelGenScript = levelGenerator.GetComponent<LevelGenerator> ();
		levelGenScript.level = levelScript.Load ();

		state = STATE.GAME;
		levelGenScript.EnableLevel ();
		return null;
	}

	private void DisplayTitle(){
		float padding = Screen.width / 50.0F;
		float titleRatio = titleTexture.width / titleTexture.height;
		Rect titleRect = new Rect (0.1F*Screen.width + 0.02F*Screen.width * Mathf.Sin(1.0F * 2.0F * Mathf.PI * Time.time), 
		                           0.1F*Screen.height + 0.02F*Screen.height * Mathf.Cos(1.0F * 2.0F * Mathf.PI * Time.time), 
		                           0.8F*Screen.width, 0.8F*Screen.width / titleRatio);
		GUI.DrawTexture (titleRect, titleTexture);
	}

	private void DisplayStartButton (){
		GUIContent startButtonContent = new GUIContent("START");
		float minWidth;
		float maxWidth;
		float startButtonHeight;
		textStyle.CalcMinMaxWidth(startButtonContent, out minWidth, out maxWidth);
		startButtonHeight = textStyle.CalcHeight (startButtonContent, maxWidth);
		Rect startButtonRect = new Rect (0.5F * Screen.width, 0.5F * Screen.height, maxWidth, startButtonHeight);
		if (GUI.Button(startButtonRect, startButtonContent, textStyle)){
			LoadLevel (0);
			state = STATE.LOADING;
		}
	}

	private void MenuOnGui(){
		DisplayTitle ();
		DisplayStartButton ();
	}
	
	//GUI management
	void OnGUI(){
		switch (state) {
		case STATE.MENU:
			MenuOnGui();
			break;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;
		textStyle = new GUIStyle ();
		textStyle.font = GameManager.getInstance ().mainFont;
		textStyle.alignment = TextAnchor.MiddleCenter;
		textStyle.normal.textColor = Color.white;
		textStyle.fontSize = 40;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameManager getInstance(){
		return instance;
	}
}
