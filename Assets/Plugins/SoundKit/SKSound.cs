using UnityEngine;
using System;
using System.Collections;


public class SKSound
{
	private SoundKit _manager;

	public AudioSource audioSource;
	public GameObject gameObject;
	public bool available = true;
	public bool destroyAfterPlay = false;
	public Action completionHandler;


	public bool loop
	{
		set
		{
			audioSource.loop = value;
		}
	}


	public SKSound( SoundKit manager, string name )
	{
		_manager = manager;

		// Create a GameObject to hold the audioSource for playing sounds
		gameObject = new GameObject();
		gameObject.name = name;
		gameObject.transform.parent = manager.transform;

		audioSource = gameObject.AddComponent<AudioSource>();
	}


	private void destroySelf()
	{
		if( _manager.removeSound( this ) )
			MonoBehaviour.Destroy( gameObject );
	}


	public void stop()
	{
		audioSource.Stop();

		if( destroyAfterPlay )
			destroySelf();
	}


	public void fadeOutAndStop( float duration, Action handler = null )
	{
		_manager.StartCoroutine
		(
			audioSource.fadeOut( duration, () =>
			{
				if( handler != null )
					handler();
				stop();
			})
		);
	}


	public void setCompletionHandler( Action handler )
	{
		completionHandler = handler;
	}


	public IEnumerator playAudioClip( AudioClip audioClip, AudioRolloffMode rolloff, float volume, Vector3 position )
	{
		// Setup the GameObject and AudioSource and start playing
		gameObject.name = audioClip.name;
		audioSource.clip = audioClip;

		return play( rolloff, volume, position );
	}


	public IEnumerator play( AudioRolloffMode rolloff, float volume, Vector3 position )
	{
		available = false;

		// Setup the GameObject and AudioSource and start playing
		gameObject.transform.position = position;

		audioSource.rolloffMode = rolloff;
		audioSource.volume = volume;
		audioSource.audio.Play();

		// Wait for the clip to finish
		yield return new WaitForSeconds( audioSource.clip.length + 0.1f );

		if( completionHandler != null )
		{
			completionHandler();
			completionHandler = null;
		}

		// Should we destroy ourself after playing?
		if( destroyAfterPlay )
			this.destroySelf();

		available = true;
	}
}
