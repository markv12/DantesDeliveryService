using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    private const string AUDIO_MANAGER_PATH = "AudioManager";
    private static AudioManager instance;
    public static AudioManager Instance {
        get {
            if (instance == null) {
                GameObject audioManagerObject = (GameObject)Resources.Load(AUDIO_MANAGER_PATH);
                GameObject instantiated = Instantiate(audioManagerObject);
                DontDestroyOnLoad(instantiated);
                instance = instantiated.GetComponent<AudioManager>();
            }
            return instance;
        }
    }

    public AudioSource dayThemeSource;
    public AudioSource nightThemeSource;
    public AudioSource shopThemeSource;

    public void PlayDayTheme() {
        dayThemeSource.volume = 0.6f;
        dayThemeSource.Play();
    }
    public void PlayNightTheme() {
        nightThemeSource.volume = 1;
        nightThemeSource.Play();
    }

    public void PlayShopTheme() {
        shopThemeSource.volume = 0.3f;
        shopThemeSource.Play();
    }

    public void FadeOutBGM() {
        if (dayThemeSource.isPlaying) {
            FadeAudioSource(dayThemeSource, 0);
        }
        if (nightThemeSource.isPlaying) {
            FadeAudioSource(nightThemeSource, 0);
        }
        if (shopThemeSource.isPlaying) {
            FadeAudioSource(shopThemeSource, 0);
        }
    }

    private void FadeAudioSource(AudioSource audioSource, float endVolume) {
        float startVolume = audioSource.volume;
        this.CreateAnimationRoutine(1.5f, (float progress) => {
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, progress);
        }, () => {
            if(endVolume <= 0) {
                audioSource.Stop();
            }
        });
    }

    [Header("Sound Effects")]
    public AudioSource[] audioSources;
    public AudioSource[] pitchedAudioSources;

    public AudioClip uiClick;
    public AudioClip nightStart;
    public AudioClip dayStart;
    public AudioClip success;
    public AudioClip[] playerHurtSounds;
    public AudioClip playerDie;
    public AudioClip outOfAmmoSound;
    public AudioClip errorSound;

    public void PlayUIClick() {
        PlaySFX(uiClick, 1f);
    }

    public void PlayNightStart() {
        PlaySFX(nightStart, 1f);
    }

    public void PlayDayStart() {
        PlaySFX(dayStart, 1f);
    }

    public void PlaySuccess() {
        PlaySFX(success, 1f);
    }

    public void PlayPlayerHurt() {
        PlaySFX(playerHurtSounds[UnityEngine.Random.Range(0, playerHurtSounds.Length)], 1f);
    }

    public void PlayPlayerDie() {
        PlaySFX(playerDie, 1f);
    }

    public void PlayOutOfAmmoSound() {
        PlaySFX(outOfAmmoSound, 1f);
    }

    public void PlayErrorSound() {
        PlaySFX(errorSound, 1f);
    }

    public void PlaySFX(AudioClip clip, float volume) {
        AudioSource source = GetNextAudioSource();
        source.volume = volume * 1;
        source.PlayOneShot(clip);
    }

    public void PlaySFXPitched(AudioClip clip, float volume, float pitch = 1) {
        AudioSource source = GetNextPitchedAudioSource();
        source.volume = volume * 1;
        source.pitch = pitch;
        source.PlayOneShot(clip);
    }

    private int audioSourceIndex = 0;
    private AudioSource GetNextAudioSource() {
        AudioSource result = audioSources[audioSourceIndex];
        audioSourceIndex = (audioSourceIndex + 1) % audioSources.Length;
        return result;
    }

    private int pitchedAudioSourceIndex = 0;
    private AudioSource GetNextPitchedAudioSource() {
        AudioSource result = pitchedAudioSources[pitchedAudioSourceIndex];
        pitchedAudioSourceIndex = (pitchedAudioSourceIndex + 1) % pitchedAudioSources.Length;
        return result;
    }
}
