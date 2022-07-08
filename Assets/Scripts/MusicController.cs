using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] public AudioClip intro, theme;
    [SerializeField] public AudioClip battle, victory, defeat;

    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        //For music "Intro + Theme"
        source.playOnAwake = false;
        source.PlayOneShot(intro);
    }

    // Update is called once per frame
    void Update()
    {
        ////For music "Intro + Theme"
        if (!source.isPlaying)
        {
            source.clip = theme;
            source.loop = true;
            source.Play();
        }
    }
}
