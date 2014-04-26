﻿using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class WordLookup : MonoBehaviour 
{
    //-----------------------------------------------------------------------------
    public bool CheckValidWord(string theWord)
    {
	    theWord = theWord.ToLower();

        // check for 2 letter word first (not long enough for 3 prefix list)
        if(theWord.Length < 2)
            return false;
	    if(theWord.Length == 2) 
            return (WordList.OtherWordList.IndexOf(theWord) != -1);

        // get the first three letters to look up in our dictionary
	    var first3 = Regex.Replace(theWord, "^(...).*", "$1");

        // are these first three letters in our table?
        string theEntry = "";
        var found = WordList.ThreeLetterPrefixWordList.TryGetValue(first3, out theEntry);
        if (!found)
        {
            return false;
        }

        theEntry = WordList.DecompressString(theEntry, first3);
       
	    var restOfWord = Regex.Replace(theWord, "^...?", "");
        return (theEntry.IndexOf("," + restOfWord + ",") != -1);
    }

    

    //-----------------------------------------------------------------------------
    public void Start()
    {
		UnitTest1();
    }

	//-----------------------------------------------------------------------------
	private void UnitTest1()
	{
		string[] wordsToTest = { "Start", "Testicle", "Vertibrate", "Fucking", "Love", "what", "qi", "x", "qwefasdf" };
		foreach (var word in wordsToTest)
		{
			Debug.Log(string.Format("Testing {0}, is word {1}, score {2}", word, CheckValidWord(word), "score?"));
		}
	}
}
