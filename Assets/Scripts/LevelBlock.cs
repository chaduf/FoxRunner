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
		get{
			return _orientation;
		}
		set{
			_orientation = value;
			this.transform.localRotation = Quaternion.AngleAxis(-60*(int)_orientation, new Vector3 (0, 0, 1));
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}
}
