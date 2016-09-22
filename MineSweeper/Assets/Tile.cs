using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public bool isBomb = false;       		// will be true if the tile is a bomb
	public float connectedDistance = 1.0f;  // max distance a neighboring tile can be
	public int chanceOfBomb = 10;       	// chance of being a bomb (out of 100%)
	public bool isFlagged = false;     		// if the tile has been flagged as a bomb
	public bool gameRunning = false;    	// used to determine if the match is running
	public Color[] numbercolors;      		// a color LUT

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
