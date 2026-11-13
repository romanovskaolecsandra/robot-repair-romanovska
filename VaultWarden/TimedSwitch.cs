using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimedSwitch : MonoBehaviour
{
    [Header("Налаштування перемикача (Inspector)")]
    [Tooltip("Час у секундах, на який відкривається перепустка")]
    public float openTime = 5.0f;

    [Tooltip("Час перезарядки кнопки після закриття")]
    public float cooldownTime = 1.0f;

    [Header("Прив'язка елементів")]
    [Tooltip("Об'єкт перепустки (ID_Card), який буде з'являтися і зникати")]
    public GameObject targetObject;

    [Tooltip("Текст на самій кнопці перемикача")]
    public TMP_Text buttonText;

    private Button switchButton;
    private float timer = 0f;
    private bool isOpen = false;
    private bool isCooldown = false;

    void Start()
    {
        switchButton = GetComponent<Button>();

        if (switchButton != null)
        {
            switchButton.onClick.AddListener(ActivateSwitch);
        }

        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        UpdateButtonVisuals("УВІМКНУТИ СКАНЕР");
    }

    void Update()
    {
        if (isOpen)
        {
            timer -= Time.deltaTime;
            UpdateButtonVisuals($"СКАНЕР АКТИВНИЙ: {timer:F1}с");

            if (timer <= 0f)
            {
                CloseSwitch();
            }
        }
        else if (isCooldown)
        {
            timer -= Time.deltaTime;
            UpdateButtonVisuals($"ОХОЛОДЖЕННЯ: {timer:F1}с");

            if (timer <= 0f)
            {
                isCooldown = false;
                if (switchButton != null) switchButton.interactable = true;
                UpdateButtonVisuals("УВІМКНУТИ СКАНЕР");
            }
        }
    }

    public void ActivateSwitch()
    {
        if (isOpen || isCooldown) return;

        isOpen = true;
        timer = openTime;

        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        if (switchButton != null)
        {
            switchButton.interactable = false;
        }
    }

    private void CloseSwitch()
    {
        isOpen = false;

        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        if (cooldownTime > 0f)
        {
            isCooldown = true;
            timer = cooldownTime;
        }
        else
        {
            if (switchButton != null) switchButton.interactable = true;
            UpdateButtonVisuals("УВІМКНУТИ СКАНЕР");
        }
    }

    private void UpdateButtonVisuals(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
        }
    }
}
