SoundKit
========

Simple pooled audio playback helper. No positional audio here or anything fancy. Directions for use:

- stick a SoundKit script on a GameObject in your scene and configure the settings in the inspector
- whenever you want to play a sound call SoundKit.instance.play
- if you don't need any control over the sound or you dont need to know when it is done playing use SoundKit.instance.playOneShot
- SoundKit.instance.play returns an SKSound object which can be used to control the AudioSource, fade out the audio over time or set a completion handler on
- to play a background music track call SoundKit.instance.playBackgroundMusic
- you can adjust volume for all the currently playing sounds via SoundKit.instance.soundEffectVolume


SoundKit will automatically pool GameObjects with AudioSources for you to avoid allocations at runtime.
