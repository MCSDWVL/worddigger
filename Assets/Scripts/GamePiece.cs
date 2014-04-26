using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour 
{
	public char Letter = 'X';
	public int Score;
	public GUIStyle Style;

	public int UsedInWordPosition { get; set; }

	public Vector2 OffsetLetter = Vector2.zero;
	public Vector2 OffsetScore = Vector2.zero;

	public float PieceSize = 64;

	// Use this for initialization
	void Start () 
	{
		UsedInWordPosition = -1;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	private void OnGUI()
	{
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.y = Screen.height - screenPos.y - PieceSize/2;
		screenPos.x -= PieceSize / 2;
		GUI.Label(new Rect(screenPos.x + OffsetLetter.x, screenPos.y + OffsetLetter.y, PieceSize, PieceSize), "" + Letter, Style);

		var pointStyle = new GUIStyle(Style);
		pointStyle.fontSize /= 2;
		GUI.Label(new Rect(screenPos.x + OffsetScore.x, screenPos.y + OffsetScore.y, PieceSize, PieceSize), "" + Score, pointStyle);
	}
}
