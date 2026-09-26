using UnityEngine;
using UnityEngine.InputSystem;
using System; // Додано для підтримки Action

public class PlayerController : MonoBehaviour
{
    // Поля класу згідно з конспектом
    public InputAction MoveAction;
    public float speed = 3.0f; // Швидкість для домашнього завдання
    Rigidbody2D rigidbody2d;
    Vector2 move;

    // Змінні здоров'я, щоб не сварилися інші скрипти
    public int maxHealth = 5;
    public int health = 5;

    // Змінна розмови, яка підійде для GameManager.cs
    public Action OnTalkedToNPC;

    void Start()
    {
        // Увімкнення вводу та отримання компонента фізики
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Зчитування натискань клавіш (ввід) виключно в Update
        move = MoveAction.ReadValue<Vector2>();
        Debug.Log(move);
    }

    void FixedUpdate()
    {
        // Физичний рух через MovePosition у FixedUpdate
        Vector2 position = (Vector2)rigidbody2d.position
            + move * speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    // Функція здоров'я, щоб сусідній скрипт Enemy.cs не видавав помилку
    public void ChangeHealth(int amount)
    {
        // Залишаємо порожньою, знадобиться в наступних юнітах
    }

    // Функція звуку, щоб скрипт HealthCollectible.cs не видавав помилку
    public void PlaySound(AudioClip clip)
    {
        // Залишаємо порожньою, звук налаштовуватиметься далі
    }
}
