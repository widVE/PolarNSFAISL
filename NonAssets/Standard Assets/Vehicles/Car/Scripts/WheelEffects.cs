using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (AudioSource))]
    public class WheelEffects : MonoBehaviour
    {
        public Transform SkidTrailPrefab;
        public static Transform skidTrailsDetachedParent;
        private ParticleSystem skidParticles;
        public bool skidding { get; private set; }
        public bool PlayingAudio { get; private set; }
        
        private AudioSource m_AudioSource;
        private Transform m_SkidTrail;
        private WheelCollider m_WheelCollider;

        //private ParticleColors particleColors;
        //private Material material;
        

        private void Start()
        {
            skidParticles = transform.root.GetComponentInChildren<ParticleSystem>();

            if (skidParticles == null)
            {
                Debug.LogWarning(" no particle system found on car to generate smoke particles");
            }
            else
            {
                skidParticles.Stop();
            }

            m_WheelCollider = GetComponent<WheelCollider>();
            m_AudioSource = GetComponent<AudioSource>();
            PlayingAudio = false;

            if (skidTrailsDetachedParent == null)
            {
                skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
            }

            //particleColors = GetComponent<ParticleColors>();
            //particleColors.SetRenderCamera();
            //material = skidParticles.GetComponent<Renderer>().material;
        }


        public void EmitTyreSmoke()
        {
            skidParticles.transform.position = transform.position - transform.up * (m_WheelCollider.radius - 0.25f);

            //material.SetColor("_Color", particleColors.GetColor());
            //particleColors.ClearRenderTexture();

            skidParticles.startSize = 3f;
            skidParticles.Emit(1);

            if (!skidding)
            {
                StartCoroutine(StartSkidTrail());
            }
        }

        // This is to remove shadowing bug when there are no particles in scene
        public void EmitTyreSmoke2()
        {
            skidParticles.transform.position = transform.position - transform.up * m_WheelCollider.radius;
            skidParticles.startSize = 0f;
            skidParticles.Emit(1);
        }


        public void PlayAudio()
        {
            m_AudioSource.Play();
            PlayingAudio = true;
        }


        public void StopAudio()
        {
            m_AudioSource.Stop();
            PlayingAudio = false;
        }


        public IEnumerator StartSkidTrail()
        {
            skidding = true;
            m_SkidTrail = Instantiate(SkidTrailPrefab);
            while (m_SkidTrail == null)
            {
                yield return null;
            }
            m_SkidTrail.parent = transform;
            m_SkidTrail.localPosition = -Vector3.up*m_WheelCollider.radius;
        }


        public void EndSkidTrail()
        {
            if (!skidding)
            {
                return;
            }
            skidding = false;
            m_SkidTrail.parent = skidTrailsDetachedParent;
            Destroy(m_SkidTrail.gameObject, 10);
        }
    }
}

