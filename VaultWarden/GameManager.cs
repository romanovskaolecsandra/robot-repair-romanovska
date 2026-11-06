using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Visitor
    {
        public string visitorName;
        [TextArea(3, 5)] public string documentDetails;
        [TextArea(3, 5)] public string dialogueLine;
        public Sprite avatarSprite;
        public bool isSpyOrIdiot;
    }

    [Header("UI Elements")]
    public TMP_Text dialogueText;
    public TMP_Text docDetailsText;
    public Image avatarImage;
    public TMP_Text livesText;
    public TMP_Text scoreText;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TMP_Text endTitleText;
    public TMP_Text endStatsText;

    [Header("Game Data")]
    public List<Visitor> visitorsList = new List<Visitor>();

    private int currentVisitorIndex = 0;
    private int score = 0;
    private int lives = 3;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (livesText != null) livesText.text = "ЩИТИ: " + new string('X', lives);
        if (scoreText != null) scoreText.text = $"БАЛИ: {score}";

        // Перевірка на поразку
        if (lives <= 0)
        {
            ShowEndScreen(false);
            return;
        }

        // Перевірка на перемогу
        if (currentVisitorIndex >= visitorsList.Count)
        {
            ShowEndScreen(true);
            return;
        }

        // Логіка показу персонажа
        Visitor currentVisitor = visitorsList[currentVisitorIndex];
        dialogueText.text = $"\"{currentVisitor.dialogueLine}\"";
        docDetailsText.text = currentVisitor.documentDetails;

        if (currentVisitor.avatarSprite != null)
        {
            avatarImage.sprite = currentVisitor.avatarSprite;
            avatarImage.color = Color.white;
        }
        else
        {
            avatarImage.color = Color.gray;
        }
    }

    public void OnAcceptButtonPressed()
    {
        if (lives <= 0 || currentVisitorIndex >= visitorsList.Count) return;

        Visitor currentVisitor = visitorsList[currentVisitorIndex];
        if (currentVisitor.isSpyOrIdiot)
        {
            lives--;
            Debug.Log("Помилка! Впущено порушника!");
        }
        else
        {
            score += 10;
            Debug.Log("Правильно! Громадянина впущено.");
        }

        NextVisitor();
    }

    public void OnRejectButtonPressed()
    {
        if (lives <= 0 || currentVisitorIndex >= visitorsList.Count) return;

        Visitor currentVisitor = visitorsList[currentVisitorIndex];
        if (currentVisitor.isSpyOrIdiot)
        {
            score += 10;
            Debug.Log("Правильно! Порушника затримано.");
        }
        else
        {
            lives--;
            Debug.Log("Помилка! Вигнали чеснулюдину!");
        }

        NextVisitor();
    }

    public void NextVisitor()
    {
        currentVisitorIndex++;
        UpdateUI();
    }

    void ShowEndScreen(bool isWin)
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);

            // Знаходимо компонент Image нашої панелі, щоб змінити її колір
            Image panelImage = endGamePanel.GetComponent<Image>();

            if (isWin)
            {
                if (panelImage != null) panelImage.color = new Color32(20, 50, 20, 255); // Темно-зелене тло

                if (endTitleText != null)
                {
                    endTitleText.text = "ЗМІНУ ЗАВЕРШЕНО";
                    endTitleText.color = Color.green;
                }

                string rank = score >= 40 ? "Ідеальний Охоронець" : "Халатний Співробітник";
                if (endStatsText != null)
                {
                    endStatsText.text = $"Ви успішно захистили Бункер-68!\n\nВаш рахунок: {score} балів.\nЗвання: {rank}";
                }
            }
            else
            {
                if (panelImage != null) panelImage.color = new Color32(60, 10, 10, 255); // Темно-червоне тло

                if (endTitleText != null)
                {
                    endTitleText.text = "БУНКЕР ЗАХОПЛЕНО";
                    endTitleText.color = Color.red;
                }
                if (endStatsText != null)
                {
                    endStatsText.text = "Бункер заражено радіоактивною пусткою через ваші помилки.\n\nСпробуйте ще раз.";
                }
            }
        }
    }
}
