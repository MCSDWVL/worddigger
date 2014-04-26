using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour 
{
	public GamePiece GamePiecePrefab;
	public char[] Letters = new char[]{'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A',	'B', 'B','C', 'C','D', 'D', 'D', 'D','E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E','F', 'F','G', 'G', 'G','H', 'H','I', 'I', 'I', 'I', 'I', 'I', 'I', 'I', 'I','J','K','L', 'L', 'L', 'L','M', 'M','N', 'N', 'N', 'N', 'N', 'N','O', 'O', 'O', 'O', 'O', 'O', 'O', 'O','P', 'P','Q','R', 'R', 'R', 'R', 'R', 'R','S', 'S', 'S', 'S','T', 'T', 'T', 'T', 'T', 'T','U', 'U', 'U', 'U','V', 'V','W', 'W','X','Y', 'Y','Z'};
	public float PieceSpacing = 1f;

	public string ActiveWord { get; private set; }
	public int Score { get; set; }

	public int Rows { get; private set; }
	public int Cols { get; private set; }

	public void OnEnable()
	{
		ActiveWord = "";
	}

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

	private GamePiece AddRandomPiece(int r, int c, System.Random rand)
	{
		var newGamePiece = GameObject.Instantiate(GamePiecePrefab.gameObject) as GameObject;
		var gamepieceScript = newGamePiece.GetComponent<GamePiece>();
		gamepieceScript.Board = this;

		// position it
		var pieceRealSize = newGamePiece.GetComponent<BoxCollider2D>().size.x;
		newGamePiece.transform.position = new Vector2(c * (pieceRealSize + PieceSpacing), -r * (pieceRealSize + PieceSpacing));

		// set the letter data
		var letter = Letters[rand.Next(0, Letters.Length)];
		gamepieceScript.Letter = letter;
		gamepieceScript.Score = ScoreLookup(letter);

		// add it to the array!
		GamePieces[c, r] = gamepieceScript;
		return gamepieceScript;
	}

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
				AddRandomPiece(r, c, _rand);
			}
		}
	}

	public void Start()
	{
		FillBoard(6, 4);
	}

	public void PieceToggled(GamePiece piece)
	{
		var alreadyInWord = piece.UsedInWord;
		if (!alreadyInWord)
		{
			piece.UsedInWordPosition = ActiveWord.Length;
			ActiveWord += piece.Letter;
			Debug.Log(ActiveWord);
		}
		else
		{
			ActiveWord = ActiveWord.Remove(piece.UsedInWordPosition);
			piece.UsedInWordPosition = -1;
			Debug.Log(ActiveWord);
		}
	}

	public GUIStyle Style;
	public Vector2 WordOffset = Vector2.zero;
	public Vector2 ScoreOffset = Vector2.zero;
	public Vector2 WordBox = Vector2.zero;

	private void OnGUI()
	{
		GUI.Label(new Rect(WordOffset.x, WordOffset.y, WordBox.x, WordBox.y), ActiveWord, Style);
		var scoreStyle = new GUIStyle(Style);
		scoreStyle.fontStyle = FontStyle.Bold;
		scoreStyle.alignment = TextAnchor.MiddleRight;
		GUI.Label(new Rect(ScoreOffset.x, ScoreOffset.y, WordBox.x, WordBox.y), "" + Score, scoreStyle);
	}

	private int ScoreActiveWord()
	{
		var score = 0;
		foreach (var letter in ActiveWord)
		{
			score += ScoreLookup(letter);
		}
		return score;
	}

	public void OnSend()
	{
		var wordlookup = GameObject.FindObjectOfType<WordLookup>();
		var isValidWord = wordlookup.CheckValidWord(ActiveWord);
		if (isValidWord)
		{
			Score += ScoreActiveWord();
			for(var c = 0; c < Cols; ++c)
			{
				for (var r = 0; r < Rows; ++r)
				{
					var piece = GamePieces[c, r];
					if (piece == null)
						continue;
					if (piece.UsedInWord)
					{
						GameObject.Destroy(piece.gameObject);

						// add preivew beneath
						var newPiece = AddRandomPiece(r, c, _rand);
						newPiece.TogglePreview();
					}
				}
			}
		}
		else
		{
			foreach (var piece in GamePieces)
			{
				if (piece == null)
					continue;

				if (piece.UsedInWord)
				{
					piece.ToggleLock();
					piece.UsedInWordPosition = -1;
				}
			}
		}

		ActiveWord = "";
	}

	public void OnDig()
	{
		ActiveWord = "";
		// lock any piece left that isn't locked, and fill the rest
		for (var c = 0; c < Cols; ++c)
		{
			for (var r = 0; r < Rows; ++r)
			{
				var piece = GamePieces[c, r];
				if (piece == null)
					AddRandomPiece(r, c, _rand);
				else if(piece.Preview)
				{
					piece.TogglePreview();
				}
				else if (!piece.Locked)
				{
					piece.UsedInWordPosition = -1;
					piece.ToggleLock();
				}
			}
		}
	}
}
