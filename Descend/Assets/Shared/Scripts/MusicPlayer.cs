using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> clips;

    private static MusicPlayer sSingleton;

    private bool mPlaying = false;
    private bool mMuted = false;
    private AudioSource mAudioSource;

    public float fadeOutTime = 0.5f;
    private float mOriginalVolume;
    private bool mFadingOut = false;
    private float mFadeOutSpeed;

    // Start is called before the first frame update
    void Start()
    {
        sSingleton = this;
        mAudioSource = GetComponent<AudioSource>();
        mOriginalVolume = mAudioSource.volume;
        mFadeOutSpeed = 1 / fadeOutTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(mFadingOut)
        {
            mAudioSource.volume -= Time.deltaTime * mFadeOutSpeed;
            if(mAudioSource.volume <= 0f)
            {
                mFadingOut = false;
                mAudioSource.Stop();
            }
        }
    }

    public static void FadeOut()
    {
        sSingleton._FadeOut();
    }
    private void _FadeOut()
    {
        mFadingOut = true;
    }

    public static void StartPlaying(int stage)
    {
        sSingleton._Start(stage);
    }
    private void _Start(int stage)
    {
        AudioClip clip = clips[Mathf.Min(clips.Count-1,stage)];
        mAudioSource.clip = clip;
        mFadingOut = false;
        mPlaying = true;
        if(!mMuted)
        {
            mAudioSource.volume = mOriginalVolume;
            mAudioSource.Play();
        }
    }

    public void ToggleMute()
    {
        if(mMuted)
        {
            mMuted = false;
            if(mPlaying)
            {
                mAudioSource.Play();
            }
        }
        else
        {
            mMuted = true;
            mAudioSource.Stop();
        }
    }
}
