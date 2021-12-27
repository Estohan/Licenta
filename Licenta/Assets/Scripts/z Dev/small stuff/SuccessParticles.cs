using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessParticles : MonoBehaviour
{
    ParticleSystem particles;

    private void Start() {
        particles = this.transform.GetComponent<ParticleSystem>();
        particles.Stop();
    }

    private void OnCollisionEnter(Collision collision) {
        particles.Play();
    }

    private void OnCollisionExit(Collision collision) {
        particles.Stop();
    }
}
