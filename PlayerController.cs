using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    private float speed = 3.0f;

    // --- ЗМІННІ ДЛЯ СУМІСНОСТІ З ІНШИМИ СКРИПТАМИ ВИКЛАДАЧА ---
    public int maxHealth = 5;
    public int health = 5;

    // Тепер це подія-делегат, як і вимагає GameManager
    public System.Action OnTalkedToNPC;

    void Start()
    {
        MoveAction.Enable();
    }

    void Update()
    {
        Vector2 move = MoveAction.ReadValue<Vector2>();

        float currentSpeed = speed;
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            currentSpeed = speed * 1.8f;
        }

        Vector2 position = (Vector2)transform.position;
        position = position + move * currentSpeed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, -10f, 10f);
        position.y = Mathf.Clamp(position.y, -10f, 10f);

        transform.position = position;
    }

    // --- ФУНКЦІЇ ДЛЯ СУМІСНОСТІ ---
    public void ChangeHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public void PlaySound(AudioClip clip)
    {
        // Порожньо для першої лабораторної
    }
}

