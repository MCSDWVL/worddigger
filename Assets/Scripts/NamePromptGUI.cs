using UnityEngine;
using System.Collections.Generic;

public class NamePromptGUI : MonoBehaviour
{
	//-------------------------------------------------------------------------
	public static string PlayerName;
	public static bool NameSet;

	private void Awake()
	{
		PlayerName = PlayerPrefs.GetString("PlayerName", "");
		if (PlayerName.Length > 0)
			NameSet = true;
		else
			PlayerName = "ENTER NAME";
	}

	public Vector2 NameEntryScale;
	public Vector2 Offset;
	public GUIStyle Style;
	private void OnGUI()
	{
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y - NameEntryScale.y / 2;
		screenPos.x -= NameEntryScale.x / 2;

		PlayerName = GUI.TextArea(new Rect(screenPos.x + Offset.x, screenPos.y + Offset.y, NameEntryScale.x, NameEntryScale.y), PlayerName, Style);
		SceneDataPasser.PlayerName = PlayerName;
	}
}