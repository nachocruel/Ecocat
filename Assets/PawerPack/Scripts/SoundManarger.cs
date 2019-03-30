using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManarger : MonoBehaviour {

    public AudioSource musicSource;
    public AudioSource efxSource;
    public AudioSource ambiteSons;
    public AudioClip clipMusicSouce;
    public static SoundManarger instance = null;
    public float lowPitchRange = .95f;              
    public float highPitchRange = 1.05f;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        musicSource.clip = clipMusicSouce;
        musicSource.loop = true;
        musicSource.Play();
        DontDestroyOnLoad(gameObject);      
    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    private void FixedUpdate()
    {
        if (Ambiente.instance.jogoFinalizado)
        {
            enabled = false;
        }
    }

    public void RandomizeSfx(AudioClip clip)
    {

        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play();
    }

    public void RandomizeSfxAmbiente(AudioClip clip)
    {

        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        ambiteSons.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        ambiteSons.clip = clip;

        //Play the clip.
        ambiteSons.Play();
    }
}
