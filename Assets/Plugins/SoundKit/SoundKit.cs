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
	private Stack<SKSound> _availableSounds;
	private List<SKSound> _playingSounds;
	[HideInInspector]
	public SKSound bgSound;


	private float _soundEffectVolume = 0.9f;
	public float soundEffectVolume
	{
		get { return _soundEffectVolume; }
		set
		{
			_soundEffectVolume = value;

			foreach( var s in _playingSounds )
				s.audioSource.volume = value;
		}
	}


	#region MonoBehaviour

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
		_availableSounds = new Stack<SKSound>( maxCapacity );
		_playingSounds = new List<SKSound>();

		for( int i = 0; i < initialCapacity; i++ )
			_availableSounds.Push( new SKSound( this, _emptyGameObjectName ) );
	}


	void OnApplicationQuit()
	{
		instance = null;
	}


	void OnLevelWasLoaded( int level )
	{
		if( dontDestroyOnLoad && clearAllAudioClipsOnLevelLoad )
		{
			for( var i = _playingSounds.Count - 1; i >= 0; i-- )
			{
				var s = _playingSounds[i];
				s.audioSource.clip = null;
				s.gameObject.name = _emptyGameObjectName;

				_availableSounds.Push( s );
				_playingSounds.RemoveAt( i );
			}
		}
	}

	#endregion


	private SKSound nextAvailableSound()
	{
		SKSound sound = null;

		if( _availableSounds.Count > 0 )
			sound = _availableSounds.Pop();

		// if we didnt find an available found, bail out
		if( sound == null )
		{
			sound = new SKSound( this, _emptyGameObjectName );
			sound.destroyAfterPlay = true;
		}

		_playingSounds.Add( sound );

		return sound;
	}


	public void playBGMusic( AudioClip audioClip, bool loop = true )
	{
		if( bgSound == null )
			bgSound = new SKSound( this, _emptyGameObjectName );

		if( loop )
			bgSound.playAudioClipLooped( audioClip, AudioRolloffMode.Linear, _soundEffectVolume, Vector3.zero );
		else
			StartCoroutine( bgSound.playAudioClip( audioClip, AudioRolloffMode.Linear, _soundEffectVolume, Vector3.zero ) );
	}


	public SKSound playSound( AudioClip audioClip, AudioRolloffMode rolloff = AudioRolloffMode.Linear, Vector3 position = default( Vector3 ) )
	{
		// Find the first SKSound not being used. if they are all in use, create a new one
		SKSound _sound = nextAvailableSound();

		StartCoroutine( _sound.playAudioClip( audioClip, rolloff, _soundEffectVolume, position ) );

		return _sound;
	}


	public SKSound playSoundLooped( AudioClip audioClip, AudioRolloffMode rolloff = AudioRolloffMode.Linear, Vector3 position = default( Vector3 ) )
	{
		// Find the first SKSound not being used. if they are all in use, create a new one
		SKSound _sound = nextAvailableSound();

		_sound.playAudioClipLooped( audioClip, rolloff, _soundEffectVolume, position );

		return _sound;
	}


	/// <summary>
	/// returns true if we are already over capacity and the SKSound should destroy it's GameObject
	/// </summary>
	/// <returns><c>true</c>, if sound was removed, <c>false</c> otherwise.</returns>
	/// <param name="s">S.</param>
	public bool removeSound( SKSound sound )
	{
		if( _availableSounds.Count + _playingSounds.Count > maxCapacity )
		{
			_playingSounds.Remove( sound );
			return true;
		}

		return false;
	}


	public void recycleSound( SKSound sound )
	{
		sound.setCompletionHandler( null );
		sound.destroyAfterPlay = false;

		_playingSounds.Remove( sound );
		_availableSounds.Push( sound );
	}

}
