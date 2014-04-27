using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour 
{
	public GamePiece GamePiecePrefab;
	public char[] Letters = new char[]{'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A',	'B', 'B','C', 'C','D', 'D', 'D', 'D','E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E','F', 'F','G', 'G', 'G','H', 'H','I', 'I', 'I', 'I', 'I', 'I', 'I', 'I', 'I','J','K','L', 'L', 'L', 'L','M', 'M','N', 'N', 'N', 'N', 'N', 'N','O', 'O', 'O', 'O', 'O', 'O', 'O', 'O','P', 'P','Q','R', 'R', 'R', 'R', 'R', 'R','S', 'S', 'S', 'S','T', 'T', 'T', 'T', 'T', 'T','U', 'U', 'U', 'U','V', 'V','W', 'W','X','Y', 'Y','Z'};
	public float PieceSpacing = 1f;

	public int LowestDepth { get; private set; }

	public int ActiveWordDepth = 0;
	public List<GamePiece> ActiveWordPieces { get; private set; }
	public string ActiveWord { get; private set; }
	public int Score { get; set; }

	public static GameBoard Instance { get; private set; }

	public int Rows { get; private set; }
	public int Cols { get; private set; }

	public GameObject ScoreBox;
	public GameObject TimeBox;

	public bool LockBadWords = false;
	public float Timer = 60;
	public float AddTimePerPointScored = 1;

	public GameObject PieceExplodeParticleObject;

	//-------------------------------------------------------------------------
	public void OnEnable()
	{
		ActiveWord = "";
		ActiveWordPieces = new List<GamePiece>();
		Instance = this;
		GamePiece.Multiplier = 1;
	}

	//-------------------------------------------------------------------------
	public int ScoreLookup(char letter)
	{
		var lower = char.ToLower(letter);
		switch (lower)
		{
			case 'a': return 1;
			case 'b': return 3;
			case 'c': return 3;
			case 'd': return 2;
			case 'e': return 1;
			case 'f': return 4;
			case 'g': return 2;
			case 'h': return 4;
			case 'i': return 1;
			case 'j': return 8;
			case 'k': return 5;
			case 'l': return 1;
			case 'm': return 3;
			case 'n': return 1;
			case 'o': return 1;
			case 'p': return 3;
			case 'q': return 10;
			case 'r': return 1;
			case 's': return 1;
			case 't': return 1;
			case 'u': return 1;
			case 'v': return 4;
			case 'w': return 4;
			case 'x': return 8;
			case 'y': return 4;
			case 'z': return 10;
			default:
				return 0;
		}
	}

	private GamePiece[,] GamePieces;

	//-------------------------------------------------------------------------
	private GamePiece AddRandomPiece(int r, int c, int depth, System.Random rand)
	{
		var newGamePiece = GameObject.Instantiate(GamePiecePrefab.gameObject) as GameObject;
		var gamepieceScript = newGamePiece.GetComponent<GamePiece>();
		gamepieceScript.Depth = depth;
		gamepieceScript.Board = this;

		// position it
		var pieceRealSize = newGamePiece.GetComponent<BoxCollider2D>().size.x;
		newGamePiece.transform.position = new Vector2(c * (pieceRealSize + PieceSpacing), -r * (pieceRealSize + PieceSpacing));

		// set the letter data
		var letter = Letters[rand.Next(0, Letters.Length)];
		gamepieceScript.Letter = letter;
		gamepieceScript.Score = ScoreLookup(letter);
		gamepieceScript.Row = r;
		gamepieceScript.Col = c;

		// add it to the array!
		GamePieces[c, r] = gamepieceScript;

		// are shades active?!
		if (IgnoreColorTime > 0)
			gamepieceScript.IgnoreDepth = true;

		// track lowest depth
		if (depth > LowestDepth)
			LowestDepth = depth;
		return gamepieceScript;
	}

	//-------------------------------------------------------------------------
	System.Random _rand = null;
	public void FillBoard(int rows, int cols, System.Random rand = null)
	{
		if(rand == null)
			rand = new System.Random();

		_rand = rand;

		Rows = rows;
		Cols = cols;

		GamePieces = new GamePiece[cols, rows];
		for (var c = 0; c < cols; ++c)
		{
			for (var r = 0; r < rows; ++r)
			{
				AddRandomPiece(r, c, 0, _rand);
			}
		}
	}

	//-------------------------------------------------------------------------
	public void Start()
	{
		FillBoard(6, 4);
	}

	//-------------------------------------------------------------------------
	public void PieceToggled(GamePiece piece)
	{
		var alreadyInWord = piece.UsedInWord;
		if (!alreadyInWord)
		{
			if (ActiveWord.Length > 0 && ActiveWordDepth != piece.Depth)
				return;
			ActiveWordDepth = piece.Depth;
			ActiveWord += piece.Letter;
			piece.UsedInWord = true;
			ActiveWordPieces.Add(piece);
		}
		else
		{
			var idx = ActiveWordPieces.IndexOf(piece);
			if(idx >= 0)
				ActiveWord = ActiveWord.Remove(idx, 1);
			ActiveWordPieces.Remove(piece);
			piece.UsedInWord = false;
		}
	}

	public GUIStyle Style;
	public Vector2 WordOffset = Vector2.zero;
	public Vector2 ScoreOffset = Vector2.zero;
	public Vector2 BoxDimensionHack = Vector2.zero;
	public Vector2 TimeOffset = Vector2.zero;

	//-------------------------------------------------------------------------
	private void OnGUI()
	{
		//Vector2 boxPosition = ScoreBox ? ScoreBox.transform.position : Vector3.zero;
		var box = TimeBox;
		Vector2 boxPosition = box ? box.transform.position : Vector3.zero;
		Vector2 screenPos = Camera.main.WorldToScreenPoint(boxPosition);
		screenPos.y = Screen.height - screenPos.y;

		var pos = screenPos + WordOffset;
		//GUI.Label(new Rect(pos.x - BoxDimensionHack.x / 2, pos.y, BoxDimensionHack.x, BoxDimensionHack.y), ActiveWord, Style);
		var scoreStyle = new GUIStyle(Style);
		scoreStyle.fontStyle = FontStyle.Bold;
		scoreStyle.alignment = TextAnchor.MiddleLeft;

		pos = screenPos + ScoreOffset;
		GUI.Label(new Rect(pos.x - BoxDimensionHack.x / 2, pos.y, BoxDimensionHack.x, BoxDimensionHack.y), "" + Score, scoreStyle);

		// time box
		box = TimeBox;
		var timeStyle = new GUIStyle(Style);
		boxPosition = box ? box.transform.position : Vector3.zero;
		screenPos = Camera.main.WorldToScreenPoint(boxPosition);
		screenPos.y = Screen.height - screenPos.y;
		pos = screenPos + TimeOffset;
		timeStyle.alignment = TextAnchor.MiddleRight;
		var minutes = (int)Timer / 60;
		var seconds = (int)Timer % 60;
		GUI.Label(new Rect(pos.x - BoxDimensionHack.x / 2, pos.y, BoxDimensionHack.x, BoxDimensionHack.y), string.Format("{0:D2}:{1:D2}", minutes, seconds), timeStyle);

		// EMERGENCY COUNTDOWN
		if (minutes == 0 && seconds < 10)
		{
			var emergencyStyle = new GUIStyle(Style);
			emergencyStyle.normal.textColor = new Color(1, 1, 1, 0.3f);
			emergencyStyle.fontSize = 500;
			emergencyStyle.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), string.Format("{0:D2}", seconds), emergencyStyle);
		}
	}

	//-------------------------------------------------------------------------
	private int ScoreActiveWord()
	{
		var score = 0;
		foreach (var letter in ActiveWord)
		{
			score += GamePiece.Multiplier * ScoreLookup(letter);
		}
		return score;
	}

	//-------------------------------------------------------------------------
	public float BombChance = 0.05f;
	public float SuperChance = 0.01f;
	public void GetRidOfPiece(GamePiece piece, int newDepth, System.Random rand)
	{
		if (piece.BeingDestroyed || piece.Fresh)
			return;

		GameObject.Destroy(piece.gameObject);
		piece.BeingDestroyed = true;
		// particle effects
		if (PieceExplodeParticleObject)
		{
			var particle = GameObject.Instantiate(PieceExplodeParticleObject, piece.gameObject.transform.position, Quaternion.identity) as GameObject;
			if (particle)
			{
				var system = particle.particleSystem;
				system.startColor = piece.RegularColor;
				system.Play();
				GameObject.Destroy(particle, system.startLifetime);
			}
		}

		// if it's a bomb destroy the neighbors!
		if (piece.Bomb)
		{
			for (var c = Mathf.Max(piece.Col - 1, 0); c <= Mathf.Min(piece.Col + 1, Cols - 1); ++c)
			{
				for (var r = Mathf.Max(piece.Row - 1, 0); r <= Mathf.Min(piece.Row + 1, Rows - 1); ++r)
				{
					Debug.Log("trying to bomb r" + r + " c" + c + "from r" + piece.Row + " c" + piece.Col);
					var bombedPiece = GamePieces[c, r];
					if (!ActiveWordPieces.Contains(bombedPiece) && !bombedPiece.BeingDestroyed)
						GetRidOfPiece(bombedPiece, newDepth, rand);
				}
			}
		}

		// add preivew beneath
		var newPiece = AddRandomPiece(piece.Row, piece.Col, newDepth, rand);
		newPiece.MatchDepthColor();
		newPiece.Fresh = true;

		var makeSuper = rand.NextDouble() < SuperChance;
		if (makeSuper)
		{
			var powers = new List<GameButton.SuperPower>(System.Enum.GetValues(typeof(GameButton.SuperPower)) as GameButton.SuperPower[]);
			powers.Remove(GameButton.SuperPower.None);
			newPiece.GrantPower = powers[rand.Next(0, powers.Count)];
		}

		var makeBomb = !makeSuper && rand.NextDouble() < BombChance;
		if (makeBomb)
		{
			Debug.Log("Making bomb");
			newPiece.Bomb = true;
		}
	}

	//-------------------------------------------------------------------------
	public void OnSend()
	{
		var wordlookup = GameObject.FindObjectOfType<WordLookup>();
		var isValidWord = wordlookup.CheckValidWord(ActiveWord);
		if (isValidWord)
		{
			var score = ScoreActiveWord();
			Score += score;
			Timer += AddTimePerPointScored * score;
			foreach (var piece in ActiveWordPieces)
			{
				if (piece == null)
					continue;

				// grant power
				if (piece.GrantPower != GameButton.SuperPower.None)
				{
					GrantPower(piece.GrantPower);
				}

				// get rid of piece
				GetRidOfPiece(piece, ActiveWordDepth+1, _rand);
			}
			Camera.main.GetComponent<CameraShaker>().Shake();
		}
		else
		{
			foreach (var piece in ActiveWordPieces)
			{
				if (piece == null)
					continue;

				// lock all the used pieces
				if (LockBadWords)
					piece.ToggleLock();

				piece.UsedInWord = false;
			}

			ActiveWordPieces.Clear();
		}

		ActiveWordPieces.Clear();
		ActiveWord = "";
	}

	//-------------------------------------------------------------------------
	public void OnDig()
	{
		Debug.LogError("Deprecated, don't think I want to do this mechanic anymore");
		ActiveWord = "";
		// lock any piece left that isn't locked, and fill the rest
		for (var c = 0; c < Cols; ++c)
		{
			for (var r = 0; r < Rows; ++r)
			{
				var piece = GamePieces[c, r];
				if (piece == null)
					AddRandomPiece(r, c, 0, _rand);
				else if(piece.Preview)
				{
					piece.TogglePreview();
				}
				else if (!piece.Locked)
				{
					piece.ToggleLock();
				}
			}
		}
	}

	public LineRenderer Line;
	public float MouseActivateDistance = .3f;
	public void Update()
	{
		if (Timer <= 0)
		{
			SceneDataPasser.LastScore = Score;
			Application.LoadLevel("ScoreBoard");
			return;
		}

		if (IgnoreColorTime > 0)
		{
			IgnoreColorTime -= Time.deltaTime;
			if (IgnoreColorTime <= 0)
			{
				foreach (var piece in GamePieces)
				{
					piece.IgnoreDepth = false;
					piece.MatchDepthColor();
				}
			}
		}

		if (ScoreMultiplyTime > 0)
		{
			ScoreMultiplyTime -= Time.deltaTime;
			if (ScoreMultiplyTime <= 0)
				GamePiece.Multiplier /= 2;
		}

		if (TimeStopTime <= 0)
			Timer -= Time.deltaTime;
		else
			TimeStopTime -= Time.deltaTime;

		if (Input.GetMouseButton(0))
		{
			var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0;

			var overlapPiece = Physics2D.OverlapPoint(mouseWorldPos);
			if (overlapPiece)
			{
				var closeEnoughToActivate = (overlapPiece.transform.position - mouseWorldPos).sqrMagnitude < MouseActivateDistance;
				if (closeEnoughToActivate)
				{
					var gamePiece = overlapPiece.GetComponent<GamePiece>();
					if (gamePiece)
					{
						if (ActiveWord.Length > 0 && ActiveWordDepth != gamePiece.Depth && !gamePiece.IgnoreDepth)
						{
							OnSend();
							return;
						}

						// HEY THEY'RE TRYING TO CHEAT!
						if (ActiveWordPieces.Count > 0)
						{
							var lastPiece = ActiveWordPieces[ActiveWordPieces.Count - 1];
							var colDif = lastPiece.Col - gamePiece.Col;
							var rowDif = lastPiece.Row - gamePiece.Row;
							if (rowDif <= -2 || rowDif >= 2 || colDif <= -2 || colDif >= 2)
							{
								OnSend();
								return;
							}
						}

						// TODO make sure user can't squeeze in the space between!
						if (ActiveWordPieces.Contains(gamePiece))
						{
							// if it's the second from the end the user is moving backwards, want to remove the last piece
							if (ActiveWordPieces.IndexOf(gamePiece) == ActiveWordPieces.Count - 2)
							{
								// remove the last piece
								ActiveWordPieces.RemoveAt(ActiveWordPieces.Count - 1);
								ActiveWord = ActiveWord.Remove(ActiveWord.Length - 1);
							}
						}
						else
						{
							ActiveWordPieces.Add(gamePiece);
							ActiveWord += gamePiece.Letter;
							ActiveWordDepth = gamePiece.Depth;
						}
					}
				}
			}

			// update the line
			Line.enabled = true;
			Line.SetVertexCount(ActiveWordPieces.Count + 1);
			for(var i = 0; i < ActiveWordPieces.Count; ++i)
				Line.SetPosition(i, ActiveWordPieces[i].transform.position + new Vector3(0,0,-1));
			Line.SetPosition(ActiveWordPieces.Count, mouseWorldPos + new Vector3(0, 0, -1));
		}
		else if (Input.GetMouseButtonUp(0))
		{
			OnSend();
			Line.enabled = false;
		}
	}

	public void ApplyAllColorPower()
	{
		// TODO want this to spread from the middle for juiciness!
		foreach (var piece in GamePieces)
		{
			piece.Depth = LowestDepth;
			piece.MatchDepthColor();
		}
	}

	public void ApplyExplodePower()
	{
		foreach (var piece in GamePieces)
			GetRidOfPiece(piece, piece.Depth + 1, _rand);
	}

	public float IgnoreColorTime = 0f;
	public float IgnoreColorPowerupSetTime = 10;
	public void ApplyIgnoreColorPower()
	{
		IgnoreColorTime = IgnoreColorPowerupSetTime;
		foreach (var piece in GamePieces)
		{
			piece.IgnoreDepth = true;
			piece.MatchDepthColor();
		}
	}

	public float ScoreMultiplyTime = 0f;
	public float ScoreMultiplyPowerupSetTime = 10;
	public void ApplyScoreMultiplierPower()
	{
		ScoreMultiplyTime = ScoreMultiplyPowerupSetTime;
		GamePiece.Multiplier *= 2;
	}

	public void ApplyShufflePower()
	{
		for (var c = 0; c < Cols; ++c)
		{
			for (var r = 0; r < Rows; ++r)
			{
				var letter = GamePieces[c, r].Letter;
				var swapRow = _rand.Next(0, Rows);
				var swapCol = _rand.Next(0, Cols);
				GamePieces[c, r].Letter = GamePieces[swapCol, swapRow].Letter;
				GamePieces[swapCol, swapRow].Letter = letter;
			}
		}
	}

	public float TimeStopTime = 0f;
	public float TimeStopTimePowerupSetTime = 10;
	public void ApplyStopTimePower()
	{
		TimeStopTime = TimeStopTimePowerupSetTime;
	}

	public void GrantPower(GameButton.SuperPower power)
	{
		var buttons = GameObject.FindObjectsOfType<GameButton>();
		foreach (var button in buttons)
			if (button.Power == power)
				button.ButtonEnabled = true;
	}
}
