using UnityEngine;

public class VentiladorGigante : MonoBehaviour
{
    public float windForce = 100f;
    public Transform windDirection; // usa el forward del ventilador
    public ParticleSystem windParticles;
    public AudioSource fanSound;

    void OnTriggerStay(Collider other)
    {
        ClienteIA clienteIA = other.GetComponentInParent<ClienteIA>();
            if (clienteIA != null)
            {
                clienteIA.ImpactadoPorItem(windForce, 10f, windDirection.forward);
            }
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.AddForce(windDirection.forward * windForce, ForceMode.Force);
            if (windParticles != null && !windParticles.isPlaying) windParticles.Play();
            if (fanSound != null && !fanSound.isPlaying) fanSound.Play();
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (windParticles != null) windParticles.Stop();
        if (fanSound != null) fanSound.Stop();
    }
}

