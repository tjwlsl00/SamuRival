using UnityEngine;
using UnityEngine.SceneManagement;

public class BlueWin_SceneManger : MonoBehaviour
{
    public void MoveToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
