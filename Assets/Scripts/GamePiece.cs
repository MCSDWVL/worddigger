using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour 
{
	public int Depth { get; set; }

	public bool Locked = false;
	public bool Preview = false;

	public Color RegularColor = new Color(1f, 0.54117647058f, 0.54117647058f, 1f);
	public Color RegularTextColor = Color.white;

	public char Letter = 'X';
	public int Score;
	public GUIStyle Style;

	public bool UsedInWord { get; set; }
	public GameBoard Board { get; set; }

	public Vector2 OffsetLetter = Vector2.zero;
	public Vector2 OffsetScore = Vector2.zero;

	public float PieceSize = 64;

	public bool IgnoreDepth { get; set; }

	public int Row { get; set; }
	public int Col { get; set; }

	public bool BeingDestroyed { get; set; }
	public bool Fresh { get; set; }

	private bool _bomb = false;
	public bool Bomb 
	{
		get { return _bomb; }
		set { _bomb = value; if (BombSprite) BombSprite.SetActive(_bomb && !Locked); }
	}
	public GameObject BombSprite;

	public static int Multiplier { get; set; }

	// grant a power?!

	private GameButton.SuperPower _power = GameButton.SuperPower.None;
	public GameButton.SuperPower GrantPower
	{
		get { return _power; }
		set
		{
			_power = value;
			if (BombSprite)
			{
				BombSprite.SetActive(_power != GameButton.SuperPower.None && !Locked);
				BombSprite.GetComponent<SpriteRenderer>().sprite = GetSpriteForPower(_power);
			}
		}
	}

	public Sprite RainbowSprite;
	public Sprite ClockSprite;
	public Sprite ShadesSprite;
	public Sprite ShuffleSprite;
	public Sprite CarrotSprite;
	public Sprite ScoreSprite;

	Sprite GetSpriteForPower(GameButton.SuperPower power)
	{
		switch (power)
		{
			case GameButton.SuperPower.AllColor:
				return RainbowSprite;
			case GameButton.SuperPower.AllExplode:
				return CarrotSprite;
			case GameButton.SuperPower.IgnoreColor:
				return ShadesSprite;
			case GameButton.SuperPower.None:
				return null;
			case GameButton.SuperPower.ScoreMultiplier:
				return ScoreSprite;
			case GameButton.SuperPower.Shuffle:
				return ShuffleSprite;
			case GameButton.SuperPower.StopTimer:
				return ClockSprite;
			default:
				return null;
		}
	}

	//-------------------------------------------------------------------------
	// Use this for initialization
	private SpriteRenderer _spriteRenderer;
	void Start () 
	{
		UsedInWord = false;
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	//-------------------------------------------------------------------------
	public void MatchDepthColor(bool setPreviousColor = false)
	{
		if (Locked)
		{
			RegularColor = Color.black;
			RegularTextColor = Color.black;
		}
		else if (!IgnoreDepth)
			RegularColor = ColorForDepth(Depth);
		else
			RegularColor = Color.grey;

		if (!_spriteRenderer)
			_spriteRenderer = GetComponent<SpriteRenderer>();
		if (setPreviousColor)
			_oldColor = ColorForDepth(Mathf.Max(Depth - 1, 0));
		else
			_oldColor = _spriteRenderer.color;

		_colorChangeTime = Time.time;

		Style.normal.textColor = RegularTextColor;
	}

	//-------------------------------------------------------------------------
	public Color[] PremadeColors;
	public bool RepeatColorsAfterOut = true;
	public bool DarkenColorsAfterOut = true;
	public float DarkenStep = 0.1f;
	public float DarkenMin = 0.1f;
	private Color ColorForDepth(int depth)
	{
		var shouldRandomize = PremadeColors == null || (depth >= PremadeColors.Length && !RepeatColorsAfterOut);

		if (shouldRandomize)
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
		else
		{
			var darkenCount = DarkenColorsAfterOut ? depth / PremadeColors.Length : 0;
			var color = PremadeColors[depth % PremadeColors.Length];
			for (var i = 0; i < darkenCount; ++i)
			{
				color.r -= DarkenStep;
				color.g -= DarkenStep;
				color.b -= DarkenStep;
			}
			return color;
		}
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
		GUI.Label(new Rect(screenPos.x + OffsetScore.x, screenPos.y + OffsetScore.y, PieceSize, PieceSize), "" + (Multiplier*Score), pointStyle);
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
			Preview = true;
		}
		else
		{
			Preview = false;
		}
	}

	//-------------------------------------------------------------------------
	public void ToggleLock()
	{
		if (!Locked)
		{
			Locked = true;
		}
		else
		{
			Locked = false;
		}
	}

	//-------------------------------------------------------------------------
	private float _colorChangeTime;
	private Color _oldColor;
	public float DefaultColorMatchSpeedMultiplier = 10;
	public float ColorMatchSpeedMultiplier = 10;
	public void Update()
	{
		Fresh = false;
		var changePct = ColorMatchSpeedMultiplier*(Time.time - _colorChangeTime);
		_spriteRenderer.color = Color.Lerp(_oldColor, RegularColor, changePct);

		if (changePct > 1)
		{
			ColorMatchSpeedMultiplier = DefaultColorMatchSpeedMultiplier;
		}
	}
}
