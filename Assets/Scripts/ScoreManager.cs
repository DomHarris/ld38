using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	[SerializeField] Text scoreText;
	[SerializeField] Text timeText;
	int score = 0;

	[SerializeField] InputHandler input;

	[SerializeField] GameObject levelUpBG;
	[SerializeField] Text levelUpText;
	[SerializeField] CameraShake shaker;
	[SerializeField] TerrainGenerator terrain;

	[SerializeField] Menu menu;

	int level = 0;
	bool regenMap = false;
	float timer = 60;

	bool stopTimer = false;
	
	// Update is called once per frame
	void Update () {
		if (!input.paused)
		{
			if (!stopTimer)
				timer -= Time.deltaTime;

			if (timer <= 0)
			{
				input.paused = true;
				menu.GameOver();
			}
			timeText.text = Mathf.RoundToInt(timer).ToString();
		}
	}

	public void UpdateScore (int scoreToAdd)
	{
		score += scoreToAdd;
		scoreText.text = score.ToString();
		CheckScore();
	}

	public void UpdateTimer (int timeToAdd)
	{
		timer += timeToAdd;
		timeText.text = Mathf.RoundToInt(timer).ToString();		
		//StartCoroutine(TimerEffect(timeToAdd));
	}

	IEnumerator TimerEffect(int timeToAdd)
	{
		float t = timer;
		stopTimer = true;

		if (timeToAdd > 0)
		{
			while (timer < t + timeToAdd)
			{
				timer++;
				yield return new WaitForEndOfFrame();
			}
		} else {
			while (timer > t + timeToAdd)
			{
				timer--;
				yield return new WaitForEndOfFrame();
			}
		}
		stopTimer = false;

	}
	
	void CheckScore ()
	{
		int lv = level;
		level = Mathf.RoundToInt(Mathf.Sqrt((100 * ((score / 2)))+50))/100;
		
		if (lv < level)
		{

			switch (level)
			{
				case 1:
					StartCoroutine (LevelUp(delegate() {
						levelUpText.text = "Level up!\n+ explosion radius";
						input.IncreaseRadius();
					}));
				break;
				case 2:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\n+ Fire Rate";
					input.IncreaseFirerate();
				}));
				break;
				case 3:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\n+ Fire Rate";
					input.IncreaseFirerate();					
				}));
				break;
				case 4:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\nExtra time!";
					UpdateTimer(30);										
				}));
				break;
				case 5:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\nRegenerating map!";
					regenMap = true;
				}));
				break;
				case 6:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\n+ Jump Height";
					input.IncreaseJumpTimer();																
				}));
				break;
				case 7:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\n+ Regenerating map and Extra time!";
					regenMap = true;
					UpdateTimer(30);										
				}));
				break;
				case 8:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\nTriple Shot!";
					input.TripleShot();
				}));
				break;
				case 9:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\n+ Fire Rate";
				}));
				break;
				case 10:
				StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\n+ Fire Rate";
				}));
				break;
				default:

				if (level % 5 == 0)
				{
					StartCoroutine (LevelUp(delegate() {
						levelUpText.text = "Level up!\nRegenerating map!";
						regenMap = true;
					}));
				} else if (level % 4 == 0)
				{
					StartCoroutine (LevelUp(delegate() {
					levelUpText.text = "Level up!\nExtra time!";
					UpdateTimer(30);										
				}));
				}
				break;
			}
		}
	}

	IEnumerator LevelUp (System.Action toExecute)
	{
		input.paused = true;
		toExecute();
		levelUpBG.SetActive(true);
		levelUpText.transform.localScale = Vector3.zero;
		LeanTween.scale (levelUpText.rectTransform, Vector3.one, 0.25f).setEase(LeanTweenType.easeInExpo);
		yield return new WaitForSeconds(.5f);
		shaker.Shake();
		if (regenMap)
		{
			yield return terrain.RegenerateMap();
			regenMap = false;
		}
		else
			yield return new WaitForSeconds(1f);
		input.paused = false;
		levelUpBG.SetActive (false);
	}

	public int GetScore ()
	{
		return score;
	}
}
