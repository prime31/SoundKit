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
	public SKSound bgSound;

	private Stack<SKSound> _availableSounds;
	private List<SKSound> _playingSounds;


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
			_availableSounds.Push( new SKSound( this ) );
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
			sound = new SKSound( this );

		_playingSounds.Add( sound );

		return sound;
	}


	public void playBackgroundMusic( AudioClip audioClip, bool loop = true )
	{
		if( bgSound == null )
			bgSound = new SKSound( this );

		if( loop )
			bgSound.playAudioClip( audioClip, _soundEffectVolume, true );
		else
			StartCoroutine( bgSound.playAudioClip( audioClip, _soundEffectVolume ) );
	}


	/// <summary>
	/// fetches any AudioSource it can find and uses the standard PlayOneShot to play. Use this if you don't require any
	/// extra control over a clip and don't care about when it completes. It avoids the call to StartCoroutine.
	/// </summary>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="volumeScale">Volume scale.</param>
	public void playOneShot( AudioClip audioClip, float volumeScale = 1f )
	{
		// find an audio source. any will work
		AudioSource source = null;

		if( _availableSounds.Count > 0 )
			source = _availableSounds.Peek().audioSource;
		else
			source = _playingSounds[0].audioSource;

		source.PlayOneShot( audioClip, volumeScale );
	}


	public SKSound playSound( AudioClip audioClip )
	{
		// Find the first SKSound not being used. if they are all in use, create a new one
		SKSound _sound = nextAvailableSound();

		StartCoroutine( _sound.playAudioClip( audioClip, _soundEffectVolume ) );

		return _sound;
	}


	/// <summary>
	/// loops the AudioClip. Do note that you are responsible for calling either stop or fadeOutAndStop on the SKSound
	/// or it will not be recycled
	/// </summary>
	/// <returns>The sound looped.</returns>
	/// <param name="audioClip">Audio clip.</param>
	public SKSound playSoundLooped( AudioClip audioClip )
	{
		// find the first SKSound not being used. if they are all in use, create a new one
		SKSound _sound = nextAvailableSound();

		_sound.playAudioClip( audioClip, _soundEffectVolume, true );

		return _sound;
	}


	/// <summary>
	/// used internally to recycle SKSounds and their AudioSources
	/// </summary>
	/// <param name="sound">Sound.</param>
	public void recycleSound( SKSound sound )
	{
		var index = 0;
		while( index < _playingSounds.Count )
		{
			if( _playingSounds[index] == sound )
				break;
			index++;
		}
		_playingSounds.RemoveAt( index );


		// if we are already over capacity dont recycle this sound but destroy it instead
		if( _availableSounds.Count + _playingSounds.Count >= maxCapacity )
			Destroy( sound.audioSource );
		else
			_availableSounds.Push( sound );
	}

}
