using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public enum GameState { Playing, Dead};
    static public GameState gameState;

	// Use this for initialization
	void Start ()
    {
        gameState = GameState.Playing;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
