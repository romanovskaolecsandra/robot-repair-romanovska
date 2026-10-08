using System;
using UnityEngine;

namespace Beginner2D
{
    public class Enemy : MonoBehaviour
    {
        public float speed = 2.0f;
        public bool vertical;
        public float changeTime = 3.0f;

        Rigidbody2D rigidbody2d;
        float timer;
        int direction = 1;
        bool broken = true;

        Animator animator;
        AudioSource audioSource;
        public AudioClip fixedSound;

        public ParticleSystem smokeParticleEffect;
        public ParticleSystem fixedParticleEffect;

        public bool isBroken { get { return broken; } }
        public event Action OnFixed;

        void Start()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            timer = changeTime;
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (!broken) return;

            timer -= Time.deltaTime;
            if (timer < 0)
            {
                direction = -direction;
                timer = changeTime;
            }
        }

        void FixedUpdate()
        {
            if (!broken) return;

            Vector2 position = rigidbody2d.position;

            if (vertical)
            {
                position.y = position.y + speed * direction * Time.deltaTime;
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", direction);
            }
            else
            {
                position.x = position.x + speed * direction * Time.deltaTime;
                animator.SetFloat("MoveX", direction);
                animator.SetFloat("MoveY", 0);
            }

            rigidbody2d.MovePosition(position);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!broken) return;

            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ChangeHealth(-1);
            }
        }

        public void Fix()
        {
            broken = false;
            rigidbody2d.simulated = false;
            animator.SetTrigger("Fixed");

            if (smokeParticleEffect != null) smokeParticleEffect.Stop();
            if (fixedParticleEffect != null) fixedParticleEffect.Play();

            if (audioSource != null && fixedSound != null)
            {
                audioSource.PlayOneShot(fixedSound);
            }

            if (OnFixed != null) OnFixed();
        }
    }
}
