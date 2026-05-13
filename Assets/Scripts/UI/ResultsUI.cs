using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ResultsUI : MonoBehaviour
{
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI wavesClearedText;
    public TextMeshProUGUI matchTimeText;
    public TextMeshProUGUI goldEarnedText;
    public TextMeshProUGUI totalGoldText;

    public Button playAgainButton;
    public Button mainMenuButton;

    public float goldCountDuration = 1.2f;

    void Start()
    {
        bool victory = GameSession.LastVictory;
        int earned = GameSession.LastGoldEarned;
        int totalGold = PlayerProgress.Gold;
        int previousGold = Mathf.Max(0, totalGold - earned);

        if (resultTitleText != null)
        {
            resultTitleText.text = victory ? "VICTORY" : "DEFEAT";
            resultTitleText.color = victory ? new Color(0.25f, 1f, 0.25f) : new Color(1f, 0.25f, 0.25f);
        }

        if (wavesClearedText != null)
            wavesClearedText.text = $"{GameSession.LastWavesCleared}";

        if (matchTimeText != null)
            matchTimeText.text = $"{FormatTime(GameSession.LastMatchTimeSeconds)}";

        if (goldEarnedText != null)
            goldEarnedText.text = $"Gold earned: +{earned}";

        if (totalGoldText != null)
            StartCoroutine(AnimateGold(previousGold, totalGold));

        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveAllListeners();
            playAgainButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("WaveMission");
            });
        }
        else
        {
            Debug.LogWarning("Play Again Button is not assigned in ResultsUI.");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });
        }
        else
        {
            Debug.LogWarning("Main Menu Button is not assigned in ResultsUI.");
        }
    }

    IEnumerator AnimateGold(int from, int to)
    {
        float t = 0f;

        while (t < goldCountDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / goldCountDuration);

            int current = Mathf.RoundToInt(Mathf.Lerp(from, to, n));

            if (totalGoldText != null)
                totalGoldText.text = $"{current}";

            yield return null;
        }

        if (totalGoldText != null)
            totalGoldText.text = $"{to}";
    }

    string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int minutes = total / 60;
        int secs = total % 60;

        return $"{minutes:00}:{secs:00}";
    }
}