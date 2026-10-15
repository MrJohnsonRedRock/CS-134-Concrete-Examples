using UnityEngine;

public class PlayOneShot : MonoBehaviour
{
    public AudioSource source;  // one AudioSource on the object
    public AudioClip clip;      // WAV for short SFX, OGG/MP3 for loops

    // Public so TriggerZone / Pickup / a Button can call it — a SOUND TRIGGER
    public void Play()
    {
        source.PlayOneShot(clip);
    }
}
