using UnityEngine;
using System.Collections;
using Completed; // This was missing in tutorial.
// https://answers.unity.com/questions/1188590/error-cs0246-in-gamemanager-roguelike-2d-tutorial.html

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

		private int level = 3;

    // Use this for initialization
    void Awake()
    {
				if (instance == null)
                    instance = this;
                else if (instance != this)
                    Destroy(gameObject);

                DontDestroyOnLoad(gameObject);
                boardScript = GetComponent<BoardManager>();
				InitGame();
    }

		void InitGame()
		{
				boardScript.SetupScene(level);
		}

    public void GameOver()
    {
      enabled = false;
    }
    // Update is called once per frame
    void Update() {
        
    }
}
