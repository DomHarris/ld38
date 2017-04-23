using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	[SerializeField] TerrainGenerator generator;
	[SerializeField] GameObject blur;

	[SerializeField] GameObject score;
	[SerializeField] GameObject time;
	[SerializeField] GameObject instructions;
	[SerializeField] GameObject gems;

	[SerializeField] GameObject gameOver;
	[SerializeField] Transform gameOverList;
	[SerializeField] GameObject gameOverItem;

	[SerializeField] InputField gameOverName;
	[SerializeField] GameObject gameOverSubmit;

	[SerializeField] ScoreManager scoreManager;

	string playerName;


	bool started = false;
	// Use this for initialization
	void Start ()
	{
		score.SetActive (false);
		time.SetActive (false);
		instructions.SetActive (true);
		gems.SetActive(false);
		gameOver.SetActive(false);
		
	}

	void Update ()
	{
		if (!started)
		{
			if (Input.anyKeyDown)
			{
				started = true;
				DeBlur();
			}
		}
	}

	public void DeBlur()
	{
		StartCoroutine(LoadGame());
	}

	IEnumerator LoadGame ()
	{
		instructions.SetActive (false);
		gems.SetActive(true);
		yield return generator.GenerateMap();
		score.SetActive (true);
		time.SetActive (true);
		blur.SetActive(false);
	}

	public void GameOver ()
	{
		gameOver.SetActive(true);
	}

	public void GameOverName ()
	{
		playerName = gameOverName.text;
		Save.LoadGame();
		Data.currentGame.scores.Add(scoreManager.GetScore(), playerName);
		foreach(KeyValuePair<int, string> score in Data.currentGame.scores.Reverse())
		{
			GameObject item = Instantiate (gameOverItem);
			item.transform.FindChild("Name").GetComponent<Text>().text = score.Value;
			item.transform.FindChild("Score").GetComponent<Text>().text = score.Key.ToString();
			item.transform.SetParent(gameOverList, false);
		}

		gameOverSubmit.SetActive(false);
		gameOverName.gameObject.SetActive(false);
		Save.SaveGame();
	}

	public void Retry ()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Exit ()
	{
		Application.Quit();
	}
}
