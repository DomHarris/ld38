using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[System.Serializable]
public class Data
{
	public static Data currentGame = new Data ( );

	[NonSerialized]
	public SortedDictionary<int, string> scores;

	[SerializeField]
	Dictionary<int, string> scoresSerializable;
	public Data ( )
	{
		scores = new SortedDictionary<int, string>();
		scoresSerializable = new Dictionary<int, string>();
	}

	public void UpdateSerializedScores()
	{
		scoresSerializable = new Dictionary<int, string>();
		foreach(KeyValuePair<int, string> score in scores )
		{
			Debug.Log(score.Value);
			scoresSerializable.Add(score.Key, score.Value);
		}
	}

	public void UpdateSortedScores()
	{
		scores = new SortedDictionary<int, string>();
		foreach(KeyValuePair<int, string> score in scoresSerializable )
		{
			scores.Add(score.Key, score.Value);
		}
	}
}

public class Save : MonoBehaviour
{
	public static Data savedData = new Data ( );

	public static void SaveGame ( )
	{
		Data.currentGame.UpdateSerializedScores();
		savedData = Data.currentGame;
		BinaryFormatter bf = new BinaryFormatter ( );
		FileStream file = File.Create ( Application.persistentDataPath + "/data" );
		bf.Serialize ( file, Save.savedData );
		file.Close ( );
	}

	public static bool LoadGame ( )
	{
		if ( File.Exists ( Application.persistentDataPath + "/data" ) )
		{
			BinaryFormatter bf = new BinaryFormatter ( );
			FileStream file = File.Open ( Application.persistentDataPath + "/data", FileMode.Open );
			
			try
			{
				Save.savedData = (Data) bf.Deserialize ( file );
				Data.currentGame = Save.savedData;
				Data.currentGame.UpdateSortedScores();
				file.Close ( );
				return true;
			} catch ( Exception e )
			{
				Debug.LogWarningFormat ( "Issue with deserialization: {0}", e );
				
				File.Delete ( Application.persistentDataPath + "/data" );
				
				file.Close ( );

				return LoadGame ( );
			}
		} else
		{
			Debug.LogWarning ( "File Not Found" );
			//Data.currentGame.modules = Module.InitModules ();
			return false;
		}
	}
}