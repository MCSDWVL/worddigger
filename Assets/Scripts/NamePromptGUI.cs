using UnityEngine;
using System.Collections.Generic;

public class NamePromptGUI : MonoBehaviour
{
	//-------------------------------------------------------------------------
	public static string PlayerName;
	public static bool NameSet;
	public static readonly string DEFAULT_NAME = "ENTER NAME";
	public GameButton StartButton;

	private void Awake()
	{
		PlayerName = PlayerPrefs.GetString("PlayerName", "");
		if (PlayerName.Length > 0)
			NameSet = true;
		else
			PlayerName = DEFAULT_NAME;
	}

	private void Update()
	{
		// lock start button until they enter a name
		if (StartButton)
			StartButton.ButtonEnabled = PlayerName != DEFAULT_NAME && PlayerName != "";
	}

	public Vector2 NameEntryScale;
	public Vector2 Offset;
	public GUIStyle Style;
	private void OnGUI()
	{
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y - NameEntryScale.y / 2;
		screenPos.x -= NameEntryScale.x / 2;

		GUI.SetNextControlName("player_name");
		PlayerName = GUI.TextArea(new Rect(screenPos.x + Offset.x, screenPos.y + Offset.y, NameEntryScale.x, NameEntryScale.y), PlayerName, Style);

		if (UnityEngine.Event.current.type == EventType.Repaint)
		{
			if (GUI.GetNameOfFocusedControl() == "player_name")
			{
				if (PlayerName == DEFAULT_NAME) PlayerName = "";
			}
			else
			{
				if (PlayerName == "") PlayerName = DEFAULT_NAME;
			}
		}

		SceneDataPasser.PlayerName = PlayerName;
	}
}