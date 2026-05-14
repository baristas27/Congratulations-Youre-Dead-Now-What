using UnityEngine;

public class DialogueAudioHandler : MonoBehaviour
{
    [Header("Typewriter Sounds")]
    [SerializeField] private AudioClip typewriterClick;
    [SerializeField] private float typewriterVolume = 0.35f;
    [SerializeField] private float pitchVariation = 0.18f;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip fileOpenSound;
    [SerializeField] private float fileOpenVolume = 0.6f;

    private AudioSource audioSource;

    private void OnEnable()
    {
        // Panel aktif olduğunda AudioSource'u garanti ediyoruz
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("[DialogueAudioHandler] AudioSource eklendi - OnEnable");
            }
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
    }

    public void PlayTypewriterClick()
    {
        if (typewriterClick == null || audioSource == null)
        {
            Debug.LogWarning("TypewriterClick AudioClip veya AudioSource null!");
            return;
        }

        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(typewriterClick, typewriterVolume);

        // Debug.Log("Typewriter sesi çalındı!");   // İstersen aç
    }

    public void PlayFileOpenSound()
    {
        if (fileOpenSound == null || audioSource == null)
        {
            Debug.LogWarning("FileOpenSound AudioClip veya AudioSource null!");
            return;
        }

        audioSource.pitch = 1f;
        audioSource.PlayOneShot(fileOpenSound, fileOpenVolume);

        // Debug.Log("File Open sesi çalındı!");
    }
}