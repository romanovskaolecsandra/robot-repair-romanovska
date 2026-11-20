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
        [TextArea(2, 4)] public string documentDetails;     // Перепустка
        [TextArea(2, 4)] public string healthCertDetails;   // Меддовідка
        [TextArea(2, 4)] public string dialogueLine;
        public Sprite avatarSprite;
        public bool isSpyOrIdiot;
    }

    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        [TextArea(2, 4)] public string dailyRules;
        public int startingLives = 3;
        public bool useHealthCert = false;
        public bool useTimer = false;
        public float timePerVisitor = 15f;
        public List<Visitor> visitors = new List<Visitor>();
    }

    [Header("Зв'язки")]
    public JuiceEffects juiceEffects;
    public MenuManager menuManager;

    [Header("Звуки")]
    public AudioSource sfxSource;
    public AudioClip successSound;
    public AudioClip errorSound;

    [Header("UI Елементи")]
    public TMP_Text rulesText;
    public TMP_Text dialogueText;
    public TMP_Text docDetailsText;
    public GameObject healthCertObject;
    public TMP_Text healthCertText;
    public Image avatarImage;
    public TMP_Text livesText;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TMP_Text endTitleText;
    public TMP_Text endStatsText;
    public GameObject nextLevelButton;

    [Header("3 Рівні гри")]
    public List<LevelData> levels = new List<LevelData>();

    private int currentLevelIndex = 0;
    private int currentVisitorIndex = 0;
    private int score = 0;
    private int lives = 3;
    private float currentTimer = 0f;
    private bool isPlaying = false;

    void Update()
    {
        if (!isPlaying || levels.Count == 0) return;

        LevelData currentLevel = levels[currentLevelIndex];

        if (currentLevel.useTimer)
        {
            currentTimer -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = $"ЧАС: {Mathf.CeilToInt(currentTimer)} с";
                timerText.color = currentTimer <= 5f ? Color.red : Color.white;
            }

            if (currentTimer <= 0f)
            {
                lives--;
                PlayError();
                if (juiceEffects != null) juiceEffects.PlayErrorFeedback();
                NextVisitor();
            }
        }
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count) return;

        currentLevelIndex = levelIndex;
        LevelData level = levels[currentLevelIndex];

        currentVisitorIndex = 0;
        score = 0;
        lives = level.startingLives;
        isPlaying = true;

        if (rulesText != null) rulesText.text = level.dailyRules;
        if (healthCertObject != null) healthCertObject.SetActive(level.useHealthCert);
        if (timerText != null) timerText.gameObject.SetActive(level.useTimer);

        UpdateUI(true);
    }

    public void StartNextLevel()
    {
        if (menuManager != null && currentLevelIndex + 1 < levels.Count)
            menuManager.SelectLevel(currentLevelIndex + 1);
        else if (menuManager != null)
            menuManager.OpenLevelSelect();
    }

    void UpdateUI(bool animateEntrance = false)
    {
        if (livesText != null) livesText.text = "ЩИТИ: " + new string('X', Mathf.Max(0, lives));
        if (scoreText != null) scoreText.text = $"БАЛИ: {score}";

        LevelData currentLevel = levels[currentLevelIndex];

        if (lives <= 0) { ShowEndScreen(false); return; }
        if (currentVisitorIndex >= currentLevel.visitors.Count) { ShowEndScreen(true); return; }

        if (currentLevel.useTimer) currentTimer = currentLevel.timePerVisitor;

        Visitor currentVisitor = currentLevel.visitors[currentVisitorIndex];
        dialogueText.text = $"\"{currentVisitor.dialogueLine}\"";
        docDetailsText.text = currentVisitor.documentDetails;

        if (currentLevel.useHealthCert && healthCertText != null)
            healthCertText.text = currentVisitor.healthCertDetails;

        if (currentVisitor.avatarSprite != null)
        {
            avatarImage.sprite = currentVisitor.avatarSprite;
            avatarImage.color = Color.white;
        }
        else
        {
            avatarImage.color = Color.gray;
        }

        if (animateEntrance && juiceEffects != null)
            juiceEffects.AnimateNewVisitor();
    }

    public void OnAcceptButtonPressed()
    {
        if (!isPlaying) return;

        Visitor currentVisitor = levels[currentLevelIndex].visitors[currentVisitorIndex];
        if (currentVisitor.isSpyOrIdiot)
        {
            lives--;
            PlayError();
            if (juiceEffects != null) juiceEffects.PlayErrorFeedback();
        }
        else
        {
            score += 10;
            PlaySuccess();
            if (juiceEffects != null) juiceEffects.PlaySuccessFeedback();
        }

        NextVisitor();
    }

    public void OnRejectButtonPressed()
    {
        if (!isPlaying) return;

        Visitor currentVisitor = levels[currentLevelIndex].visitors[currentVisitorIndex];
        if (currentVisitor.isSpyOrIdiot)
        {
            score += 10;
            PlaySuccess();
            if (juiceEffects != null) juiceEffects.PlaySuccessFeedback();
        }
        else
        {
            lives--;
            PlayError();
            if (juiceEffects != null) juiceEffects.PlayErrorFeedback();
        }

        NextVisitor();
    }

    void PlaySuccess()
    {
        if (sfxSource != null && successSound != null)
            sfxSource.PlayOneShot(successSound);
    }

    void PlayError()
    {
        if (sfxSource != null && errorSound != null)
            sfxSource.PlayOneShot(errorSound);
    }

    public void NextVisitor()
    {
        currentVisitorIndex++;
        UpdateUI(true);
    }

    void ShowEndScreen(bool isWin)
    {
        isPlaying = false;

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);

            if (isWin)
            {
                int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
                if (currentLevelIndex + 1 > unlockedLevel)
                {
                    PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 1);
                    PlayerPrefs.Save();
                }

                if (endTitleText != null)
                {
                    endTitleText.text = "ЗМІНУ ЗАВЕРШЕНО";
                    endTitleText.color = Color.green;
                }

                int maxScore = levels[currentLevelIndex].visitors.Count * 10;
                string rank = score >= maxScore - 10 ? "Ідеальний Охоронець" : "Втомлений Наглядач";

                if (endStatsText != null)
                    endStatsText.text = $"Рівень «{levels[currentLevelIndex].levelName}» пройдено!\n\nРахунок: {score} / {maxScore}\nЗвання: {rank}";

                if (nextLevelButton != null)
                    nextLevelButton.SetActive(currentLevelIndex + 1 < levels.Count);
            }
            else
            {
                if (endTitleText != null)
                {
                    endTitleText.text = "БУНКЕР ЗАХОПЛЕНО";
                    endTitleText.color = Color.red;
                }
                if (endStatsText != null)
                    endStatsText.text = "Бункер заражено радіоактивною пусткою через ваші помилки.\n\nСпробуйте ще раз.";

                if (nextLevelButton != null) nextLevelButton.SetActive(false);
            }
        }
    }
}
