using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour 
{
	public int Depth { get; set; }

	public bool Locked = false;
	public bool Preview = false;

	public Color RegularColor = new Color(1f, 0.54117647058f, 0.54117647058f, 1f);
	public Color RegularTextColor = Color.white;

	public Color PreviewColor = new Color(0.36862745098f, 0.36862745098f, 0.36862745098f, 1f);
	public Color PreviewTextColor = new Color(0.56862745098f, 0.56862745098f, 0.56862745098f, 1f);

	public Color LockColor = Color.black;
	public Color LockTextColor = Color.grey;

	public char Letter = 'X';
	public int Score;
	public GUIStyle Style;

	public bool UsedInWord { get; set; }
	public GameBoard Board { get; set; }

	public Vector2 OffsetLetter = Vector2.zero;
	public Vector2 OffsetScore = Vector2.zero;

	public float PieceSize = 64;

	public int Row { get; set; }
	public int Col { get; set; }

	//-------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		UsedInWord = false;
	}

	//-------------------------------------------------------------------------
	public void MatchDepthColor()
	{
		RegularColor = ColorForDepth(Depth);
		var spriteRend = GetComponent<SpriteRenderer>();
		spriteRend.color = RegularColor;
		Style.normal.textColor = RegularTextColor;
	}

	//-------------------------------------------------------------------------
	private Color ColorForDepth(int depth)
	{
		var rand = new System.Random(depth);

		// don't want it to be too white, so let's say we have to distribute .75+.75+.75 between our three colors
		var colorBudget = .75f * 3;
		var red = Mathf.Min((float)rand.NextDouble() * colorBudget, 1);
		colorBudget -= red;
		var blue = Mathf.Min((float)rand.NextDouble() * colorBudget, 1);
		colorBudget -= blue;
		var green = (float)rand.NextDouble() * colorBudget;

		return new Color(red, green, blue);
	}

	//-------------------------------------------------------------------------
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

	//-------------------------------------------------------------------------
	public bool ToggleOnClick = false;
	private void OnMouseDown()
	{
		if (Locked || Preview || !ToggleOnClick)
			return;

		Board.PieceToggled(this);

		// do something to make it visible that this letter is selected!
		if (UsedInWord)
			Style.fontStyle = FontStyle.BoldAndItalic;
		else
			Style.fontStyle = FontStyle.Normal;
	}

	//-------------------------------------------------------------------------
	public void TogglePreview()
	{
		if (!Preview)
		{
			var spriteRend = GetComponent<SpriteRenderer>();
			spriteRend.color = PreviewColor;
			Style.normal.textColor = PreviewTextColor;
			Preview = true;
		}
		else
		{
			var spriteRend = GetComponent<SpriteRenderer>();
			spriteRend.color = RegularColor;
			Style.normal.textColor = RegularTextColor;
			Preview = false;
		}
	}

	//-------------------------------------------------------------------------
	public void ToggleLock()
	{
		if (!Locked)
		{
			var spriteRend = GetComponent<SpriteRenderer>();
			spriteRend.color = LockColor;
			Style.normal.textColor = LockTextColor;
			Locked = true;
		}
		else
		{
			var spriteRend = GetComponent<SpriteRenderer>();
			spriteRend.color = RegularColor;
			Style.normal.textColor = RegularTextColor;
			Locked = false;
		}
	}
}
