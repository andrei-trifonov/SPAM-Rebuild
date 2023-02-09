using UnityEngine;
using System.Collections;
using System;

public class AudioFadeOut
{
    public static IEnumerator FadeOut(AudioSource sound, float fadingTime, Func<float, float, float, float> Interpolate)
    {
        float startVolume = sound.volume;
        float frameCount = fadingTime / Time.deltaTime;
        float framesPassed = 0;

        while (framesPassed <= frameCount)
        {
            var t = framesPassed++ / frameCount;
            sound.volume = Interpolate(startVolume, 0, t);
            yield return null;
        }

        sound.volume = 0;
        sound.Pause();
    }
    public static IEnumerator FadeIn(AudioSource sound, float fadingTime, Func<float, float, float, float> Interpolate)
    {
        sound.Play();
        sound.volume = 0;

        float resultVolume = sound.volume;
        float frameCount = fadingTime / Time.deltaTime;
        float framesPassed = 0;

        while (framesPassed <= frameCount)
        {
            var t = framesPassed++ / frameCount;
            sound.volume = Interpolate(0, resultVolume, t);
            yield return null;
        }

        sound.volume = resultVolume;
    }
}