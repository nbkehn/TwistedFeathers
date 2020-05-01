using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using https://answers.unity.com/questions/1260393/make-music-continue-playing-through-scenes.html
//as a reference
public class Music : MonoBehaviour
{
    private AudioSource source;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        source = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (source.isPlaying) return;
        source.Play();
    }

    public void StopMusic()
    {
        source.Stop();
    }
}