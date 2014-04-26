﻿using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour 
{
	public GamePiece GamePiecePrefab;
	public char[] Letters = new char[]{'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A',	'B', 'B','C', 'C','D', 'D', 'D', 'D','E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E','F', 'F','G', 'G', 'G','H', 'H','I', 'I', 'I', 'I', 'I', 'I', 'I', 'I', 'I','J','K','L', 'L', 'L', 'L','M', 'M','N', 'N', 'N', 'N', 'N', 'N','O', 'O', 'O', 'O', 'O', 'O', 'O', 'O','P', 'P','Q','R', 'R', 'R', 'R', 'R', 'R','S', 'S', 'S', 'S','T', 'T', 'T', 'T', 'T', 'T','U', 'U', 'U', 'U','V', 'V','W', 'W','X','Y', 'Y','Z'};
	public float PieceSpacing = 1f;

	public int ActiveWordDepth = 0;
	public List<GamePiece> ActiveWordPieces { get; private set; }
	public string ActiveWord { get; private set; }
	public int Score { get; set; }

	public int Rows { get; private set; }
	public int Cols { get; private set; }

	public GameObject ScoreBox;

	public void OnEnable()
	{
		ActiveWord = "";
		ActiveWordPieces = new List<GamePiece>();
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
				AddRandomPiece(r, c, 0, _rand);
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
			Debug.Log(ActiveWord);
		}
	}

	public GUIStyle Style;
	public Vector2 WordOffset = Vector2.zero;
	public Vector2 ScoreOffset = Vector2.zero;
	public Vector2 WordBox = Vector2.zero;

	private void OnGUI()
	{
		Vector2 boxPosition = ScoreBox ? ScoreBox.transform.position : Vector3.zero;
		Vector2 screenPos = Camera.main.WorldToScreenPoint(boxPosition);
		screenPos.y = Screen.height - screenPos.y;

		var pos = screenPos + WordOffset;
		GUI.Label(new Rect(pos.x - WordBox.x / 2, pos.y, WordBox.x, WordBox.y), ActiveWord, Style);
		var scoreStyle = new GUIStyle(Style);
		scoreStyle.fontStyle = FontStyle.Bold;
		scoreStyle.alignment = TextAnchor.MiddleRight;

		pos = screenPos + ScoreOffset;
		GUI.Label(new Rect(pos.x - WordBox.x / 2, pos.y, WordBox.x, WordBox.y), "" + Score, scoreStyle);
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
			foreach (var piece in ActiveWordPieces)
			{
				if (piece == null)
					continue;
				if (piece.UsedInWord)
				{
					GameObject.Destroy(piece.gameObject);

					// add preivew beneath
					var newPiece = AddRandomPiece(piece.Row, piece.Col, ActiveWordDepth + 1, _rand);
					newPiece.MatchDepthColor();
				}
			}
		}
		else
		{
			foreach (var piece in ActiveWordPieces)
			{
				if (piece == null)
					continue;

				// lock all the used pieces
				if (piece.UsedInWord)
					piece.ToggleLock();

				piece.UsedInWord = false;
			}

			ActiveWordPieces.Clear();
		}

		ActiveWordPieces.Clear();
		ActiveWord = "";
	}

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
}
