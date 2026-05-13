using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSquadSetup()
    {
        SceneManager.LoadScene("SquadSetup");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMission()
    {
        SceneManager.LoadScene("Mission");
    }
}
