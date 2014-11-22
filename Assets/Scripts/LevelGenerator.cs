using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class LevelGenerator : MonoBehaviour {

	private static LevelGenerator instance;

	public struct LEVEL{
		public GameObject startBlockPrefab;
		public GameObject[] blockPrefabs;
		public float startScrollingSpeed;
		public float speedAcceleration;
		public float minScrollingSpeed;
		public float maxScrollingSpeed;
		public float scrollingSpeedInc;
		public float scrollingSpeedDec;
	}

	public enum STATE
	{
		DISABLED,
		TRANSITION,
		SCROLLING,
		STARTING
	};

	public LEVEL level;
	
	private ArrayList createdBlocks;
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
	public Texture2D coinTexture;
	public Texture2D lifeTexture;

	private GameObject lastBlock = null;
	private GameObject[] genLevelBlocks;

	public int coinStep;
	private int coinTarget;

	public float speedChangePeriod;
	private float lastSpeedChange;

	//Utilities functions

	private void DestroyLevel(){
		foreach (GameObject block in createdBlocks) {
			Destroy(block);		
		}
		createdBlocks.Clear ();
	}

	private void ManageScrollingSpeed(){
		Hero heroScript = hero.GetComponent<Hero> ();

		if (Time.time - lastSpeedChange > speedChangePeriod){
			scrollingSpeed = Mathf.Min (scrollingSpeed + level.scrollingSpeedInc, level.maxScrollingSpeed);
			lastSpeedChange = Time.time;
		}

		if (heroScript.coin > coinTarget){
			int accelCoef = (coinTarget - heroScript.coin)/coinStep + 1;
			scrollingSpeed = Mathf.Max (scrollingSpeed - (float)accelCoef * level.scrollingSpeedDec, level.minScrollingSpeed);
			coinTarget += accelCoef * coinStep;
		}
	}

	private void MoveBlocks (){
		ArrayList blocksToRemoveIndex = new ArrayList();

		ManageScrollingSpeed ();

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
			int index = Random.Range (0, level.blockPrefabs.Length);
			LevelBlock.ORIENTATION orientation = (LevelBlock.ORIENTATION)Random.Range(0, 6);
			CreateBlock(level.blockPrefabs[index], orientation);
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
			StartTransition(false);
		}
		if (Input.GetKey(KeyCode.RightArrow) && heroState == Hero.STATE.RUNNING){
			StartTransition(true);
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
		float step = ((LevelBlock)level.startBlockPrefab.GetComponent<LevelBlock>()).size;
		CreateBlock (level.startBlockPrefab, 
		             startLevelBlockOrientation, 
		             endPosition, false);

		for (float z=endPosition + step; z>startPosition; z+=step) {
			CreateBlock(level.startBlockPrefab, startLevelBlockOrientation, z, false);
		}

		hero.GetComponent<Hero>().Wait();
		state = STATE.STARTING;
		startingTime = 0.0F;

		coinTarget = coinStep;
		scrollingSpeed = level.startScrollingSpeed;
		lastSpeedChange = Time.time;
	}

	private void RestartLevel(){
		DestroyLevel ();
		StartLevel ();
	}

	private void StopLevel (){
		DestroyLevel ();
		state = STATE.DISABLED;
	}

	private void WaitStart (){
		startingTime += Time.deltaTime;

		if (startingTime > startingDuration) {
			state = STATE.SCROLLING;
			hero.GetComponent<Hero> ().StartLevel();
		}
	}

	private void checkHeroState(){
		Hero heroScript = (Hero)hero.GetComponent<Hero> ();
		switch (heroScript.state){
			case Hero.STATE.DEAD:
			if (heroScript.life < 1){
				DisableLevel();
				GameManager.GetInstance().state = GameManager.STATE.GAME_OVER;
				return;
			}
			if (state == STATE.SCROLLING)
				RestartLevel();
				break;
		}
	}

	private void DisplayLife(){
		Hero heroScript = hero.GetComponent <Hero>();
		float screenFraction = Screen.width/10;
		float padding = Screen.width / 50;
		float line = 0.0F;
		float col = 0.0F;
		for (int life = 0; life < heroScript.life; life+=1){
			Rect lifeTexturePos = new Rect(col * screenFraction + padding, line * screenFraction + padding,
			                               screenFraction, screenFraction);
			GUI.DrawTexture(lifeTexturePos, lifeTexture);

			if (col * screenFraction > Screen.width/2.0F - padding){
				col = 0.0F;
				line += 1.0F;
			}
			else {
				col += 1.0F;
			}
		}
	}

	private void DisplayCoin(){
		float screenFraction = Screen.width/10;
		float padding = Screen.width / 50;
		string coinText = hero.GetComponent <Hero> ().coin.ToString ();
		Rect coinTexturePos = new Rect (Screen.width - screenFraction - padding, padding, screenFraction, screenFraction);
		Rect coinTextPos;
		GUIStyle coinTextStyle = new GUIStyle ();
		GUIContent content;
		float minWidth;
		float maxWidth;

		GUI.DrawTexture(coinTexturePos, coinTexture);

		coinTextStyle.font = GameManager.GetInstance ().mainFont;
		coinTextStyle.alignment = TextAnchor.MiddleLeft;
		coinTextStyle.normal.textColor = Color.white;
		coinTextStyle.fontSize = 40;

		content = new GUIContent (coinText);
		coinTextStyle.CalcMinMaxWidth (content, out minWidth, out maxWidth);
		coinTextPos = new Rect (Screen.width - screenFraction - maxWidth - padding, 
		                        padding, screenFraction * 2.0F, screenFraction);
		GUI.Label (coinTextPos, coinText, coinTextStyle);
	}

	// Runtime functions

	private void StartingUpdate(){
		Scroll (level.startBlockPrefab, startLevelBlockOrientation);
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

	private void DisabledUpdate(){

	}

	private void StartingOnGUI(){
		GUI.color = Color.Lerp (Color.black, 
                                new Color(0, 0, 0, 0.0F), 
                                startingTime/startingDuration);

		GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), transitionScreen);

		GUI.color = Color.white;
	}

	private void ScrollingOnGUI (){
		DisplayLife();
		DisplayCoin ();
	}

	private void TransitionOnGUI (){
		DisplayLife();
		DisplayCoin ();
	}

	void Start ()
	{
		instance = this;
		createdBlocks = new ArrayList ();

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
		case STATE.DISABLED:
			DisabledUpdate();
			break;
		}
	}

	void OnGUI(){
		switch (state) 
		{
		case STATE.SCROLLING:
			ScrollingOnGUI ();
			break;
		case STATE.TRANSITION:
			TransitionOnGUI();
			break;
		case STATE.STARTING:
			StartingOnGUI();
			break;
		}
	}

	//Public functions

	public void loadLevel(LEVEL newlevel){
		level = newlevel;
	}
	
	public void DisableLevel(){
		DestroyLevel ();
		Destroy (hero);
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

	public void StartTransition(bool right){
		if (right){
			hero.GetComponent<Hero> ().ChangeRoad ();
			currentOrientation = (--currentOrientation) % 6;
			if (currentOrientation < 0)
				currentOrientation += 6;
			state = STATE.TRANSITION;	
		}
		else {
			hero.GetComponent<Hero>().ChangeRoad();
			currentOrientation = (++currentOrientation)%6;
			state = STATE.TRANSITION;
		}
	}

	public static LevelGenerator GetInstance()
	{
		return instance;
	}
}
