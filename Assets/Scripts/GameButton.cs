using UnityEngine;
using System.Collections;

public class GameButton : MonoBehaviour
{
	public string Text = "";

	// this is terrible don't do it
	public bool isSendButton = false;
	public bool isDigButton = false;
	
	public GUIStyle Style;
	public Vector2 Offset = Vector2.zero;
	public Vector2 ButtonScale; 

	private void OnGUI()
	{
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y - ButtonScale.y / 2;
		screenPos.x -= ButtonScale.x / 2;
		GUI.Label(new Rect(screenPos.x + Offset.x, screenPos.y + Offset.y, ButtonScale.x, ButtonScale.y), "" + Text, Style);
	}

	// more terrible code
	private GameBoard _board;
	private void OnMouseDown()
	{
		_board = GameObject.FindObjectOfType<GameBoard>();
		if (!_board)
		{
			Debug.LogError("Oops can't find board!");
			return;
		}
		if (isSendButton)
		{
			_board.OnSend();
		}
		else if (isDigButton)
		{
			_board.OnDig();
		}
	}
}
