using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SoundKit : MonoBehaviour
{
	public static SoundKit instance = null;
	public int initialCapacity = 10;
	public int maxCapacity = 15;
	public bool dontDestroyOnLoad = true;
	public bool clearAllAudioClipsOnLevelLoad = true;

	private const string _emptyGameObjectName = "No Sound";
	private List<SKSound> _soundList;
	[HideInInspector]
	public SKSound bgSound;


	private float _soundEffectVolume = 0.9f;
	public float soundEffectVolume
	{
		get { return _soundEffectVolume; }
		set
		{
			_soundEffectVolume = value;

			foreach( var s in _soundList )
			{
				if( !s.available )
					s.audioSource.volume = value;
			}
		}
	}



	void Awake()
	{
		// avoid duplicates
		if( instance != null )
		{
			Destroy( gameObject );
			return;
		}

		instance = this;

		if( dontDestroyOnLoad )
			DontDestroyOnLoad( gameObject );

		// Create the _soundList to speed up sound playing in game
		_soundList = new List<SKSound>( initialCapacity );

		for( int i = 0; i < initialCapacity; i++ )
			_soundList.Add( new SKSound( this, _emptyGameObjectName ) );
	}


	void OnApplicationQuit()
	{
		instance = null;
	}


	void OnLevelWasLoaded( int level )
	{
		if( dontDestroyOnLoad && clearAllAudioClipsOnLevelLoad && _soundList != null )
		{
			foreach( var s in _soundList )
			{
				s.audioSource.clip = null;
				s.gameObject.name = _emptyGameObjectName;
			}
		}
	}


	private SKSound nextAvailableSound()
	{
		SKSound sound = null;

		foreach( var s in _soundList )
		{
			// dont care about sounds that arent available
			if( !s.available )
				continue;

			sound = s;
		}

		// if we didnt find an available found, bail out
		if( sound == null )
		{
			sound = new SKSound( this, _emptyGameObjectName );
			sound.destroyAfterPlay = true;
			_soundList.Add( sound );
		}

		return sound;
	}


	public void playBGMusic( AudioClip audioClip, bool loop = false )
	{
		if( bgSound == null )
			bgSound = new SKSound( this, _emptyGameObjectName );

		bgSound.loop = loop;
		StartCoroutine( bgSound.playAudioClip( audioClip, AudioRolloffMode.Linear, _soundEffectVolume, Vector3.zero ) );
	}


	public SKSound playSound( AudioClip audioClip, AudioRolloffMode rolloff = AudioRolloffMode.Linear, Vector3 position = default( Vector3 ) )
	{
		// Find the first SKSound not being used. if they are all in use, create a new one
		SKSound _sound = nextAvailableSound();

		StartCoroutine( _sound.playAudioClip( audioClip, rolloff, _soundEffectVolume, position ) );

		return _sound;
	}


	/// <summary>
	/// returns true if we are already over capacity and the SKSound should destroy it's GameObject
	/// </summary>
	/// <returns><c>true</c>, if sound was removed, <c>false</c> otherwise.</returns>
	/// <param name="s">S.</param>
	public bool removeSound( SKSound s )
	{
		if( _soundList.Count > maxCapacity )
		{
			_soundList.Remove( s );
			return true;
		}

		return false;
	}

}
