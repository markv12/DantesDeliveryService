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

    public void PlayDayTheme() {
        dayThemeSource.volume = 1;
        dayThemeSource.Play();
    }
    public void PlayNightTheme() {
        nightThemeSource.volume = 1;
        nightThemeSource.Play();
    }
    public void FadeOutBGM() {
        if (dayThemeSource.isPlaying) {
            FadeAudioSource(dayThemeSource);
        }
        if (nightThemeSource.isPlaying) {
            FadeAudioSource(nightThemeSource);
        }
    }

    private void FadeAudioSource(AudioSource audioSource) {
        float startVolume = audioSource.volume;
        this.CreateAnimationRoutine(1.5f, (float progress) => {
            audioSource.volume = Mathf.Lerp(startVolume, 0, progress);
        }, () => {
            audioSource.Stop();
        });
    }

    [Header("Sound Effects")]
    public AudioSource[] audioSources;

    private int audioSourceIndex = 0;

    public AudioClip uiClick;
    public AudioClip nightStart;
    public AudioClip dayStart;

    public void PlayUIClick() {
        PlaySFX(uiClick, 1f);
    }

    public void PlayNightStart() {
        PlaySFX(nightStart, 1f);
    }

    public void PlayDayStart() {
        PlaySFX(dayStart, 1f);
    }

    public void PlaySFX(AudioClip clip, float volume, float pitch = 1) {
        AudioSource source = GetNextAudioSource();
        source.volume = volume * 1;
        source.pitch = pitch;
        source.PlayOneShot(clip);
    }

    private AudioSource GetNextAudioSource() {
        AudioSource result = audioSources[audioSourceIndex];
        audioSourceIndex = (audioSourceIndex + 1) % audioSources.Length;
        return result;
    }
}
