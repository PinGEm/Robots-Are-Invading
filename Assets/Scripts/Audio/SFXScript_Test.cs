using UnityEngine;

public class SFXScript_Test : MonoBehaviour
{
    public static SFXScript_Test instance;

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
}
