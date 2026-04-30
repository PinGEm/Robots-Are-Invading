using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1f)
    {
        // GameObject
        AudioSource audiosource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // AudioClip
        audiosource.clip = audioClip;

        // Volume
        audiosource.volume = volume;

        // Play Sound
        audiosource.Play();

        // Length of Audio Clip
        float clipLength = audiosource.clip.length;

        // Destroy Audio Clip after Playing
        Destroy(audiosource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume = 1f)
    {
        int rand_clip = Random.Range(0, audioClips.Length);
        float rand_pitch = Random.Range(0.85f, 1.15f);

        AudioSource audiosource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        
        audiosource.clip = audioClips[rand_clip];
        audiosource.volume = volume;
        audiosource.pitch = rand_pitch;

        audiosource.Play();

        float clipLength = audiosource.clip.length;

        Destroy(audiosource.gameObject, clipLength);
    }
}
