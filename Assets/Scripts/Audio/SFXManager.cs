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

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
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

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClips.Length);

        // GameObject
        AudioSource audiosource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // AudioClip
        audiosource.clip = audioClips[rand];

        // Volume
        audiosource.volume = volume;

        // Play Sound
        audiosource.Play();

        // Length of Audio Clip
        float clipLength = audiosource.clip.length;

        // Destroy Audio Clip after Playing
        Destroy(audiosource.gameObject, clipLength);
    }
}
