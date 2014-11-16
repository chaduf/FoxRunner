using UnityEngine;
using System.Collections;

public class LevelBlock : MonoBehaviour {
	public enum ORIENTATION{
		DOWN,
		DOWN_LEFT,
		UP_LEFT,
		UP,
		UP_RIGHT,
		DOWN_RIGHT
	};

	public static float speed;
	public Vector3 endPoint;
	public float size;
	private ORIENTATION _orientation;
	
	public ORIENTATION orientation
	{
		get
		{
			return _orientation;
		}
		set
		{
			_orientation = value;
			this.transform.localRotation = Quaternion.AngleAxis(-60*(int)_orientation, new Vector3 (0, 0, 1));
		}
	}


//	// parse the ressource text to build an orientation
//	private GameObject parseResource(string text){
//		int x = 0, y = 0, z = 0;
//		float blockScale = 1;
//		GameObject block;
//		
//		foreach (char c in text){
//			if (c=='X'){
//				Vector3 prefabPosition = new Vector3(blockScale*Mathf.Sin(60*x), blockScale*Mathf.Cos(60*x), 0);
//				Instantiate(platformPrefab, prefabPosition, Quaternion.AngleAxis(60*x));
//				y++;
//			}
//			else if ('\n'){
//				x++;
//				y=0;
//			}
//			else if(c==';'){
//				x=0;
//				y=0;
//				z++;
//			}
//			else{
//				
//			}
//		}
//		return block;
//	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3(0,0,-speed*Time.deltaTime));

		if (transform.position.z < endPoint.z) {
			Debug.Log("LevelBlock destroy");
			Destroy(this.gameObject);	
		}
	}
}
