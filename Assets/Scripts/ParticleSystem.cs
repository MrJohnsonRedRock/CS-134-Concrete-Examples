using UnityEngine;

public class PlayParticles : MonoBehaviour
{
    // Drag your ParticleSystem here in the Unity Inspector
    public ParticleSystem hitEffect;

    // Call this method to trigger the particle burst
    public void PlayEffect()
    {
        if (hitEffect != null)
        {
            hitEffect.Play();   // plays the ParticleSystem once
        }
    }


}