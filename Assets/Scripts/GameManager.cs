using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public enum STATE{
		MENU,
		LEVEL_SELECT,
		GAME,
		PAUSE,
		LOADING,
		GAME_OVER
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

	private void DisplayGameOver(){
		float padding = Screen.width / 50.0F;
		float gameOverRatio = gameOverTexture.width / gameOverTexture.height;
		Rect gameOverRect = new Rect (0.1F*Screen.width + 0.1F*Screen.width * Mathf.Sin(0.25F * 2.0F * Mathf.PI * Time.time), 
		                              0.2F*Screen.height + 0.2F*Screen.height * Mathf.Cos(0.25F * 2.0F * Mathf.PI * Time.time), 
		                              0.8F*Screen.width, 0.8F*Screen.width / gameOverRatio);
		GUI.DrawTexture (gameOverRect, gameOverTexture);
	}

	private bool DisplayButton(string text, float left, float top){
		GUIContent content = new GUIContent(text);
		float minWidth;
		float maxWidth;
		float buttonHeight;
		textStyle.CalcMinMaxWidth(content, out minWidth, out maxWidth);
		buttonHeight = textStyle.CalcHeight (content, maxWidth);
		Rect buttonRect = new Rect (left* Screen.width, top * Screen.height, maxWidth, buttonHeight);

		return GUI.Button (buttonRect, content, textStyle);
	}

	private void DisplayStartButton (){
		if (DisplayButton("Start", 0.4F, 0.6F)){
			LoadLevel (0);
			state = STATE.LOADING;
		}
	}

	private void DisplayLvSelectButton(){
		if (DisplayButton("Select level", 0.4F, 0.7F)){
			state = STATE.LEVEL_SELECT;
		}
	}

	public void DisplayQuitButton(){
		if (DisplayButton("Quit", 0.4F, 0.8F)){
			Application.Quit();
		}
	}

	private void DispQuitRetButtons(){
		if (DisplayButton("Return", 0.2F, 0.8F)){
			state = STATE.MENU;
		}
		
		if (DisplayButton("Quit", 0.2F, 0.9F)){
			Application.Quit();
		}
	}

	private void DisplayLevelSelect(){
		Vector2 scrollPosition = Vector2.zero;
		float interspace = 50.0F;
		Rect container = new Rect (0.1F * Screen.width, 0.1F * Screen.height, 0.9F * Screen.width, 0.7F * Screen.height);
		Rect viewRect = new Rect(0,0, 0.9F*Screen.width, levels.Length*interspace);
		
		for (int i=0; i<levels.Length; i++) {
			GUIContent quitButtonContent = new GUIContent("Quit");
		}
		scrollPosition = GUI.BeginScrollView (container, scrollPosition, viewRect);
		for (int i=0; i<levels.Length; i++) {
			Rect levelButtonRect = new Rect(0, i*interspace, viewRect.width, interspace);
			GUIContent levelButtonContent = new GUIContent("Level " + (i+1));
			if (GUI.Button (levelButtonRect, levelButtonContent, textStyle)) {
				LoadLevel(i);
			}
		}
		GUI.EndScrollView();
	}

	private void MenuOnGui(){
		DisplayTitle ();
		DisplayStartButton ();
		DisplayLvSelectButton ();
		DisplayQuitButton ();
	}

	private void LevelSelectOnGUI () {
		DisplayLevelSelect ();
		DispQuitRetButtons ();
	}

	private void GameOverOnGui(){
		DisplayGameOver ();
		DispQuitRetButtons ();
	}

	//GUI management
	void OnGUI(){
		switch (state) {
		case STATE.MENU:
			MenuOnGui();
			break;
		
		case STATE.LEVEL_SELECT:
			LevelSelectOnGUI();
			break;
		

		case STATE.GAME_OVER:
			GameOverOnGui();
			break;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;
		textStyle = new GUIStyle ();
		textStyle.font = GameManager.GetInstance ().mainFont;
		textStyle.alignment = TextAnchor.MiddleCenter;
		textStyle.normal.textColor = Color.white;
		textStyle.fontSize = 40;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameManager GetInstance(){
		return instance;
	}
}
