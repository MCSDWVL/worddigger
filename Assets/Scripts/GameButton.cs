using UnityEngine;
using System.Collections;

public class GameButton : MonoBehaviour
{
	public string Text = "";

	// this is terrible don't do it
	public bool isSendButton = false;
	public bool isDigButton = false;
	public bool isPlayButton = false;
	public bool isResetButton = false;
	public bool isSuperButton = false;

	public enum SuperPower { None, AllColor, AllExplode, StopTimer, ScoreMultiplier, IgnoreColor, Shuffle };

	public bool ButtonEnabled = true;
	
	public GUIStyle Style;
	public Vector2 Offset = Vector2.zero;
	public Vector2 ButtonScale; 

	private void OnGUI()
	{
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y - ButtonScale.y / 2;
		screenPos.x -= ButtonScale.x / 2;
		if (!ButtonEnabled)
			Style.normal.textColor = Color.grey;
		else
			Style.normal.textColor = Color.white;
		GUI.Label(new Rect(screenPos.x + Offset.x, screenPos.y + Offset.y, ButtonScale.x, ButtonScale.y), "" + Text, Style);
	}

	// more terrible code
	private GameBoard _board;
	private void OnMouseDown()
	{
		if (!ButtonEnabled)
			return;

		if (isSendButton)
		{
			_board.OnSend();
		}
		else if (isDigButton)
		{
			_board.OnDig();
		}
		else if (isPlayButton)
		{
			if (NamePromptGUI.PlayerName.Length > 0)
			{
				NamePromptGUI.NameSet = true;
				PlayerPrefs.SetString("PlayerName", NamePromptGUI.PlayerName);
				Application.LoadLevel("MainGame");
			}
		}
		else if (isResetButton)
		{
			Application.LoadLevel("MainGame");
		}
		else if (isSuperButton)
		{
		}
	}
}
