using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameBoard : MonoBehaviour {

	public Text timerText;
	public float timerStart = 0;
	public bool timerRunning = false;

	// Use this for initialization
	void Start () {
		timerText.text = "Time: 0";
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(timerRunning){
			timerText.text = "Time: " + (int)(Time.realtimeSinceStartup - timerStart);
		}
	}

	public void StartTimer(){
		timerRunning = true;
		timerStart = Time.realtimeSinceStartup;
		BroadcastMessage("BeginGame");
	}

	public void StopTimer(){
		timerRunning = false; 
		BroadcastMessage("EndGame");
	}

	public void GameOver(){
		Tile[] tiles = FindObjectsOfType<Tile>();
		foreach(Tile t in tiles){
			if(t.isBomb){
				t.Reveal(); 
				StopTimer();
			}
		}
	}

	public void TestWinCondition(){
		bool pass = true;
		Tile[] tiles = FindObjectsOfType<Tile>();
		foreach(Tile t in tiles){
			if(t.isBomb && !t.isFlagged){
				pass = false;
			}
		}

		if(pass){
			StopTimer();
			Debug.Log("YOU ARE WINRAR");
		}
	}
}
