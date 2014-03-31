using UnityEngine;
using System;
using System.Collections;


public class SKSound
{
	private SoundKit _manager;

	public AudioSource audioSource;
	public Action completionHandler;


	public SKSound( SoundKit manager )
	{
		_manager = manager;
		audioSource = _manager.gameObject.AddComponent<AudioSource>();
	}


	private IEnumerator fadeOut( float duration, Action onComplete )
	{
		var startingVolume = audioSource.volume;

		// fade out the volume
		while( audioSource.volume > 0.0f )
		{
			audioSource.volume -= Time.deltaTime * startingVolume / duration;
			yield return null;
		}

		// all done fading out
		if( onComplete != null )
			onComplete();
	}


	public void stop()
	{
		audioSource.Stop();

		if( completionHandler != null )
		{
			completionHandler();
			completionHandler = null;
		}

		_manager.recycleSound( this );
	}


	public void fadeOutAndStop( float duration, Action handler = null )
	{
		_manager.StartCoroutine
		(
			fadeOut( duration, () =>
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


	public IEnumerator playAudioClip( AudioClip audioClip, float volume )
	{
		playAudioClip( audioClip, volume, false );

		// wait for the clip to finish (this is a best guess estimate)
		yield return new WaitForSeconds( audioSource.clip.length + 0.1f );

		stop();
	}


	public void playAudioClip( AudioClip audioClip, float volume, bool shouldLoop )
	{
		// setup the GameObject and AudioSource and start playing
		audioSource.clip = audioClip;
		audioSource.loop = shouldLoop;
		audioSource.volume = volume;
		audioSource.Play();
	}

}
