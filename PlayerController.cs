using UnityEngine;
using UnityEngine.InputSystem;

namespace Beginner2D
{
    public class PlayerController : MonoBehaviour
    {
        public InputAction MoveAction;
        public InputAction LaunchAction;
        public float speed = 4.0f;
        public int maxHealth = 5;
        public float timeInvincible = 2.0f;
        public GameObject projectilePrefab;

        int currentHealth;
        bool isInvincible;
        float damageCooldown;
        float shotCooldown = 0.5f;
        float shotTimer;

        Rigidbody2D rigidbody2d;
        Vector2 move;
        Vector2 moveDirection = new Vector2(1, 0);
        Animator animator;

        public int health { get { return currentHealth; } }

        void Start()
        {
            MoveAction.Enable();
            LaunchAction.Enable();
            rigidbody2d = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;
        }

        void Update()
        {
            move = MoveAction.ReadValue<Vector2>();

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                moveDirection.Set(move.x, move.y);
                moveDirection.Normalize();
            }

            animator.SetFloat("Look X", moveDirection.x);
            animator.SetFloat("Look Y", moveDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            if (isInvincible)
            {
                damageCooldown -= Time.deltaTime;
                if (damageCooldown < 0) isInvincible = false;
            }

            if (shotTimer > 0) shotTimer -= Time.deltaTime;

            if (LaunchAction.WasPressedThisFrame() && shotTimer();
            if (proj != null)
            {
                proj.Launch(moveDirection, 300f);
            }
            animator.SetTrigger("Launch");
        }
    }
}
