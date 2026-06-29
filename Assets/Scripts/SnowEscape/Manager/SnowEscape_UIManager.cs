using UnityEngine;
using System.Collections;

public class SnowEscape_UIManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] GameObject gameRederingPanel;

    private void InitialUISetting()
    {
        gameRederingPanel.SetActive(false);
    }

    IEnumerator Start()
    {
        InitialUISetting();
        yield return new WaitForSeconds(5.5f);
        gameRederingPanel.SetActive(true);
    }


}