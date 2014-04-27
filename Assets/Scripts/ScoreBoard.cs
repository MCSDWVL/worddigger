using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreBoard : MonoBehaviour
{
	private string _scoreString = "";
	public GUIStyle Style;
	private List<string> _displayNames = new List<string>();
	private List<string> _displayScores = new List<string>();

	private string _topNamesWithLineReturn = "";
	private string _topScoresWithLineReturn = "";

	public void PostScore(string name, int score)
	{
		var www = new WWW("http://dreamlo.com/lb/uKbhpJEoek2e5v_fd-MeYwBVgk_4TMY0--Y6r6B9zbxA/add-pipe/" + name + "/" + score);
		StartCoroutine(WaitForRequest(www));
		_scoreString = "LOADING...";
		//yield www;
		// assign texture
		//renderer.material.mainTexture = www.texture;
	}

	public void Start()
	{
		if (SceneDataPasser.PlayerName != "")
		{
			Debug.Log("posting score");
			PostScore(SceneDataPasser.PlayerName, SceneDataPasser.LastScore);
		}
		else
		{
			Debug.Log("sending request");
			var www = new WWW("http://dreamlo.com/lb/535c5bc16e51b42adcada1eb/pipe");
			StartCoroutine(WaitForRequest(www));
			_scoreString = "LOADING...";
		}
	}

	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;

		_displayNames.Clear();
		_displayScores.Clear();
		_topNamesWithLineReturn = "";
		_topScoresWithLineReturn = "";

		// check for errors
		if (www.error == null)
		{
			_scoreString = www.text;
			var split = _scoreString.Split('\n');
			var scores = 0;
			foreach (var nameAndScore in split)
			{
				var parts = nameAndScore.Split('|');
				if (parts.Length >= 2)
				{
					var name = parts[0];
					var score = parts[1];
					_displayNames.Add(name);
					_displayScores.Add(score);

					if (scores < 20)
					{
						_topNamesWithLineReturn += name + "\n";
						_topScoresWithLineReturn += score + "\n";
					}
				}
				++scores;
			}

			_topNamesWithLineReturn += "--------------\n";
			_topScoresWithLineReturn += "--------------\n";

			_topNamesWithLineReturn += SceneDataPasser.PlayerName;
			_topScoresWithLineReturn +=  SceneDataPasser.LastScore;
			Debug.Log("WWW Ok!: " + www.text);
		}
		else
		{
			_displayNames.Add("ERROR");
		}
	}

	public float BoardScreenWidth = .80f;
	public void OnGUI()
	{
		GUI.TextField(new Rect(Screen.width * (1-BoardScreenWidth)/2, 0, Screen.width * BoardScreenWidth, Screen.height), _topNamesWithLineReturn, Style);

		var scoreStyle = new GUIStyle(Style);
		scoreStyle.alignment = TextAnchor.UpperRight;
		GUI.TextField(new Rect(Screen.width * (1 - BoardScreenWidth)/2, 0, Screen.width * BoardScreenWidth, Screen.height), _topScoresWithLineReturn, scoreStyle);
	}
}

