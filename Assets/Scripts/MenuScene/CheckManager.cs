using UnityEngine;
using System.Collections;

public class CheckManager : MonoBehaviour
{
    [SerializeField] GameObject[] players;
    private MenuPlayer menuRed;
    private MenuPlayer menuBlue;
    // bool 
    public bool isAllPlayerReady = false;
    // 외부 
    [SerializeField] GameObject snowsplater;
    private SnowSplat snowSplat;

    void Awake()
    {
        menuRed = players[0].GetComponent<MenuPlayer>();
        menuBlue = players[1].GetComponent<MenuPlayer>();
        snowSplat = snowsplater.GetComponent<SnowSplat>();
    }

    public void CheckMenuPlayerReady()
    {
        if (!isAllPlayerReady) return;
        menuRed.PlayThrowSnow();
        menuBlue.PlayThrowSnow();
        StartCoroutine(WaitforThrow());
    }

    IEnumerator WaitforThrow()
    {
        yield return new WaitForSeconds(1.5f);
        snowSplat.VisibleSnowSplat();
    }
}
