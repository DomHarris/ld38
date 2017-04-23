using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	[SerializeField] AudioClip explosion;
	[SerializeField] AudioClip powerup;
	[SerializeField] AudioClip pickup;
	[SerializeField] AudioClip hit;

	[SerializeField] AudioSource source;

	public void PlaySound (string sound)
	{
		AudioClip clip = null;
		float volume = 0.5f;
		switch (sound)
		{
			case "explosion" : clip = explosion; volume = 0.1f; break;
			case "powerup" : clip = powerup; break;
			case "pickup" : clip = pickup; break;
			case "hit" : clip = hit; break;
		}

		source.PlayOneShot(clip, volume);
	}
}
