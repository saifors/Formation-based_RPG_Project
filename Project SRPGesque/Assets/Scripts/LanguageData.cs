﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

//[System.Serializable]
public class LanguageData
{
	public enum Language { English, Spanish, Dutch, German };
	public Language currentLanguage;

	//Diccionarios con textos
	public Dictionary<string, string> attackName; //First string is the key could also be an int to indicate ID number, second could be something else like a GameObject assigned to a key or ID
	public Dictionary<string, string> attackDesc;

	//--------------------

	public LanguageData() //Detect the PCs language to initialize it as the PC language
	{
		SystemLanguage systemLanguage = Application.systemLanguage;

		if (systemLanguage == SystemLanguage.Spanish) currentLanguage = Language.Spanish;
		else if (systemLanguage == SystemLanguage.Dutch) currentLanguage = Language.Dutch;
		else if (systemLanguage == SystemLanguage.German) currentLanguage = Language.German;		
		else currentLanguage = Language.English;

		Debug.Log("System language = " + systemLanguage);
		Debug.Log("Current language = " + currentLanguage);
		attackName = new Dictionary<string, string>();
		attackDesc = new Dictionary<string, string>();
	}
}

public static class LanguageManager
{
	public static LanguageData langData; // static so the value can be accessed from any part in any code

	public static void LoadLanguage()
	{
		langData = new LanguageData();

		LoadAttackNames();
		LoadAttackDesc();
	}

	public static void LoadAttackNames() //This isnt an universal void, there needs to be one for each file.
	{
		try
		{
			langData.attackName = new Dictionary<string, string>(); //reinitializing because like this it gets overwritten every time its loaded which is helpful when the language is changed during runtime.

			string fullText = DataManager.LoadTextFromFile("TextData/AttackNames"); //No need to put Resources as it´s already loading from inside Resources 
																 //Debug.Log(fullText);
																 //Now we need to seperate the text into lines so:
			string[] linesText = DataManager.ReadLinesFromString(fullText);
			//Debug.Log(linesText[0]);
			for (int i = 1; i < linesText.Length; i++) //i is 1 because line 0 is not info for the program
			{
				//langData.form //Dictionary, it´s already loaded inside Language Data
				string[] rows = linesText[i].Split('\t'); //Splits it by tabs inside of the string.

				switch (langData.currentLanguage)
				{
					case LanguageData.Language.English:
						langData.attackName.Add(rows[0], rows[1]);
						break;
					case LanguageData.Language.Spanish:
						langData.attackName.Add(rows[0], rows[2]);
						break;
					case LanguageData.Language.Dutch:
						langData.attackName.Add(rows[0], rows[3]);
						break;
					case LanguageData.Language.German:
						langData.attackName.Add(rows[0], rows[4]);
						break;
					default:
						break;
				}
			}

		}
		catch (Exception e)
		{
			Debug.LogError("Load from error: " + e);
		}
	}
	public static void LoadAttackDesc() //This isnt an universal void, there needs to be one for each file.
	{
		try
		{
			langData.attackDesc = new Dictionary<string, string>(); //reinitializing because like this it gets overwritten every time its loaded which is helpful when the language is changed during runtime.

			string fullText = DataManager.LoadTextFromFile("TextData/AttackDescriptions"); //No need to put Resources as it´s already loading from inside Resources 
																					//Debug.Log(fullText);
																					//Now we need to seperate the text into lines so:
			string[] linesText = DataManager.ReadLinesFromString(fullText);
			//Debug.Log(linesText[0]);
			for (int i = 1; i < linesText.Length; i++) //i is 1 because line 0 is not info for the program
			{
				//langData.form //Dictionary, it´s already loaded inside Language Data
				string[] rows = linesText[i].Split('\t'); //Splits it by tabs inside of the string.

				switch (langData.currentLanguage)
				{
					case LanguageData.Language.English:
						langData.attackDesc.Add(rows[0], rows[1]);
						break;
					case LanguageData.Language.Spanish:
						langData.attackDesc.Add(rows[0], rows[2]);
						break;
					case LanguageData.Language.Dutch:
						langData.attackDesc.Add(rows[0], rows[3]);
						break;
					case LanguageData.Language.German:
						langData.attackDesc.Add(rows[0], rows[4]);
						break;
					default:
						break;
				}
			}

		}
		catch (Exception e)
		{
			Debug.LogError("Load from error: " + e);
		}
	}

}