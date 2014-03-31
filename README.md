SoundKit
========

Simple pooled audio playback helper. No positional audio here or anything fancy. SoundKit will automatically pool AudioSources for you to avoid allocations at runtime. In fact, the only time it allocates anything at all (outside of Awake when it creates the pool) is when you play more than *maxCapacity* AudioClips simultaneously.


Directions for use:

- stick a SoundKit script on a GameObject in your scene and configure the settings in the inspector
- whenever you want to play a sound call *SoundKit.instance.play*
- if you don't need any control over the sound or you dont need to know when it is done playing use *SoundKit.instance.playOneShot*


Some notes about more advanced usage:

- *SoundKit.instance.play* returns an SKSound object which can be used to control the AudioSource, fade out the audio over time or set a completion handler to be notified when playback is complete
- you can adjust volume for all the currently playing sounds via *SoundKit.instance.soundEffectVolume*
- to play a background music track call *SoundKit.instance.playBackgroundMusic* (only one background music track can be played at a time and it is recommended to use the Stream from disc option for these)
- you can adjust individual volume for any playing sound or background music by just accessing the *SKSound.audioSource* directly
- to loop any AudioClip call the *setLoop* method on the SKSound class. Note that if you turn looping on you are responsible for calling *stop* to recycle the SKSound!
