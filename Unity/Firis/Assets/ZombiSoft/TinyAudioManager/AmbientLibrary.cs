//==============================================================
// Sound Library (Ambient)
//==============================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbientLibrary : MonoBehaviour 
{
	public AmbientGroup[] ambientGroups;

	Dictionary<string, AudioClip> groupDictionary = new Dictionary<string, AudioClip>();

	void Awake() 
	{
		foreach (AmbientGroup ambientGroups in ambientGroups)
		{
			groupDictionary.Add (ambientGroups.name, ambientGroups.clip);
		}
	}

	public AudioClip GetClipFromName(string name) 
	{
		if (groupDictionary.ContainsKey (name)) 
		{
			AudioClip ambient = groupDictionary [name];
			return ambient;
		}
		return null;
	}

	[System.Serializable]
	public class AmbientGroup
	{
		public string name;
		public AudioClip clip;
	}
}