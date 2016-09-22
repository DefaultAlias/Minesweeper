using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public bool isBomb = false;       		// will be true if the tile is a bomb
	public float connectedDistance = 1.0f;  // max distance a neighboring tile can be
	public int chanceOfBomb = 10;       	// chance of being a bomb (out of 100%)
	public bool isFlagged = false;      	// if the tile has been flagged as a bomb
	public bool gameRunning = false;    	// used to determine if the match is running
	public Color[] numbercolors;      		// a color LUT

	List<Tile> connectedTiles = new List<Tile>(); // a list of all neighboring tiles
	TextMesh text;                  			  // the display text of this tile
	bool isRevealed = false;            		  // will be true if the tile is shown


	// Use this for initialization
	void Start () {
		// get the TextMesh object for use later
		text = transform.GetComponentInChildren<TextMesh>();
	}

	// starts a new match
	public void BeginGame(){
		// start the match
		gameRunning = true;

		// reset all values
		isRevealed = false;
		isFlagged = false;
		GetComponent<Renderer>().material.color = Color.white;
		text.text = "";
		isBomb = false;

		// determine if we're a bomb
		int bombRand = Random.Range(0, 100);
		if(bombRand <= chanceOfBomb){
			isBomb = true;
		}

		// connect to nearest tiles
		connectedTiles.Clear();
		Tile[] tiles = FindObjectsOfType<Tile>();
		foreach(Tile t in tiles){
			// if the tile is in range, add it to our neighbors
			if(Vector3.Distance(transform.position, t.transform.position) < connectedDistance){
				connectedTiles.Add(t);
			}
		}
	}

	// Ends the match and stops all input
	public void EndGame(){
		gameRunning = false;
	}

	// Update is called once per frame
	void Update () {
		// aim text at camera
		text.transform.LookAt(Camera.main.transform);
		text.transform.Rotate(new Vector3(0, 180, 0));
	}

	// Called when the mouse is over our object
	void OnMouseOver () {
		if(gameRunning){
			// reveal the tile on left click
			if(Input.GetMouseButtonDown(0)){
				if(!isRevealed){
					Reveal();

					// if the tile has 0 bombs, clear area around it
					if (getBombCount() == 0){
						ClearNeigbors();
					}
					// if the count < 0, it's a bomb and we've lost
					else if (getBombCount() < 0){
						SendMessageUpwards("GameOver");
					}
				}
			}
			// flag the tile on right click
			if(Input.GetMouseButtonDown(1)){
				Flag();
			}
		}
	}

	// Returns the number of bombs around this tile, or -1 if the tile is a bomb
	public int getBombCount (){
		if(isBomb){
			return -1;
		}
		else {
			int bombsInProximity = 0;
			foreach(Tile t in connectedTiles){
				if(t.isBomb){
					bombsInProximity ++;
				}
			}
			return bombsInProximity;
		}
	}

	// Coroutine that spins the tile 180 degrees
	IEnumerator Spin() {
		for (float f = 0.0f; f < 180.0f; f += 10f) {
			transform.RotateAround(transform.position, transform.up, 10);
			yield return null;
		}
	}

	// Reveals the contents of a tile
	public void Reveal(){
		// set material to cleared material
		if(!isRevealed && gameRunning){
			// call the coroutine that will update the rotation of the tile
			StartCoroutine(Spin());

			// set tile text and materials
			int bombCount = getBombCount();
			if(bombCount < 0){
				text.text = "X";
				text.color = Color.Lerp(Color.black, numbercolors[15], 0.5f);
				Color matColor = Color.Lerp(Color.white, numbercolors[15], 0.75f);
				GetComponent<Renderer>().material.color = matColor;
			}
			else if(bombCount == 0){
				text.text = "";
				text.color = Color.Lerp(Color.black, numbercolors[0], 0.5f);
				Color matColor = Color.Lerp(Color.white, numbercolors[0], 0.75f);
				GetComponent<Renderer>().material.color = matColor;
			}
			else {
				text.text = "" + bombCount;
				text.color = Color.Lerp(Color.black, numbercolors[bombCount], 0.5f);
				Color matColor = Color.Lerp(Color.white, numbercolors[bombCount], 0.75f);
				GetComponent<Renderer>().material.color = matColor;
			}

			// keep track of if the tile was revealed
			isRevealed = true;
		}
	}

	// Flags a tile as a potential bomb
	void Flag(){
		// you can only flag a tile if it's hidden
		// and we can only toggle a flag off if a tile is already a flag
		if(!isRevealed || isFlagged){
			isFlagged = !isFlagged; // toggle the flagged variable

			if(isFlagged){
				// spin the tile around
				StartCoroutine(Spin());

				// send a message to the GameStateManager above this 
				// in the hierarchy to test for win condition
				SendMessageUpwards("TestWinCondition");

				// change the text and materials
				text.text = "F";
				text.color = Color.Lerp(Color.black, numbercolors[14], 0.5f);
				Color matColor = Color.Lerp(Color.white, numbercolors[14], 0.75f);
				GetComponent<Renderer>().material.color = matColor;

			}
			else {
				// spin the tile around
				StartCoroutine(Spin());

				// set the text and materials back to default
				text.text = "";
				text.color = Color.Lerp(Color.black, numbercolors[14], 0.5f);
				Color matColor = Color.Lerp(Color.white, numbercolors[14], 0.75f);
				GetComponent<Renderer>().material.color = matColor;
			}
		}
	}
	// Clears all empty neighboring tiles
	void ClearNeigbors(){
		// for all tiles around this tile..
		foreach(Tile t in connectedTiles){
			// if the tile is empty, has no bombs around it, and is not revealved,
			// reveal it and also reveal its neighbors the same way
			if(t.getBombCount() == 0 && !t.isRevealed){
				t.Reveal();
				t.ClearNeigbors();
			}
			// if the tile does have bombs around it, but is not a bomb, show it too
			else if(t.getBombCount() > 0){
				t.Reveal();
			}
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.gray;
		Gizmos.color = Color.red;
		foreach(Tile t in connectedTiles){
			Gizmos.DrawLine(transform.position, t.transform.position);
		}
	}
}
