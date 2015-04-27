using UnityEngine;
using System.Collections;


public class SoundKitDemo : MonoBehaviour
{
	public AudioClip explosion;
	public AudioClip fart;
	public AudioClip rocket;
	public AudioClip squish;
	public AudioClip windBGSound;

	private float _volume = 0.9f;
	private float _bgMusicVolume = 0.9f;
	private SoundKit.SKSound _loopedFartSound;


	public void OnGUI()
	{
		if( GUILayout.Button( "Play Explosion" ) )
			SoundKit.instance.playSound( explosion );

		if( GUILayout.Button( "Play Explosion via One Shot" ) )
			SoundKit.instance.playOneShot( explosion );

		GUILayout.Space( 20 );

		if( GUILayout.Button( "Play Fart" ) )
			SoundKit.instance.playSound( fart );

		if( _loopedFartSound == null )
		{
			if( GUILayout.Button( "Play Fart Looped" ) )
				_loopedFartSound = SoundKit.instance.playSound( fart ).setLoop( true );
		}
		else
		{
			if( GUILayout.Button( "Stop Looping Fart Sound" ) )
			{
				_loopedFartSound.stop();
				_loopedFartSound = null;
			}
		}

		if( GUILayout.Button( "Play Fart with Completion Handler" ) )
		{
			SoundKit.instance.playSound( fart )
				.setCompletionHandler( () => Debug.Log( "done playing fart" ) );
		}

		GUILayout.Space( 20 );

		if( GUILayout.Button( "Play Rocket" ) )
			SoundKit.instance.playSound( rocket );

		if( GUILayout.Button( "Play Squish" ) )
			SoundKit.instance.playSound( squish );

		if( GUILayout.Button( "Play Wind Background Audio" ) )
			SoundKit.instance.playBackgroundMusic( windBGSound, 8f, true );
		
		if( GUILayout.Button( "Stop Background Audio" ) )
			SoundKit.instance.backgroundSound.stop();

		if( GUILayout.Button( "Toggle AudioListener.pause" ) )
			AudioListener.pause = !AudioListener.pause;


		GUILayout.Label( "Sound Effect Volume" );

		var oldVolume = _volume;
		_volume = GUILayout.HorizontalSlider( _volume, 0f, 1f );
		if( oldVolume != _volume )
			SoundKit.instance.soundEffectVolume = _volume;


		GUILayout.Space( 20 );
		if( SoundKit.instance.backgroundSound != null )
		{
			GUILayout.Label( "BG Music Volume" );

			oldVolume = _bgMusicVolume;
			_bgMusicVolume = GUILayout.HorizontalSlider( _bgMusicVolume, 0f, 1f );
			if( oldVolume != _bgMusicVolume )
				SoundKit.instance.backgroundSound.audioSource.volume = _bgMusicVolume;
		}
	}

}
