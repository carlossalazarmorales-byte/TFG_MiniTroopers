using UnityEngine;
using TMPro;

public class MainMenuGoldUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        if (goldText != null)
            goldText.text = $"{PlayerProgress.Gold}";
    }
}
