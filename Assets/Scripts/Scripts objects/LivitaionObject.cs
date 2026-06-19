using UnityEngine;

public class LivitaionObject : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particles;
    private void FixedUpdate()
    {
        if (_particles != null && !_particles.isPlaying)
            _particles.Play();
    }
}
