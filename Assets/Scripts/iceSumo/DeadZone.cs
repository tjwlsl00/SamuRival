using System.Collections;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    // bool
    private bool isRedFallen = false;
    private bool isBlueFallen = false;
    private bool isRoundOver = false;
    private bool isJudging = false;

    public void PlayerFallen(string playerTag)
    {
        if (isRoundOver) return;

        if (playerTag == "Red") isRedFallen = true;
        if (playerTag == "Blue") isBlueFallen = true;

        if (!isJudging)
        {
            StartCoroutine(JudgeResultRoutine());
        }
    }

    IEnumerator JudgeResultRoutine()
    {
        isJudging = true;

        yield return new WaitForSeconds(0.05f);

        isRoundOver = true;

        if (isRedFallen && isBlueFallen)
        {
            Debug.Log("무승부");

            // UI 효과
            IceSumo_UIManager.Instance.VisibleDrawPanel();

            // 사운드 효과
            IceSumo_SoundManager.Instance.PlayDrawClip();

            // 리붓 함수 호출 
            IceSumo_RoundManager.Instance.RebootRound();
        }
        else if (isRedFallen)
        {
            IceSumo_RoundManager.Instance.IncreaseScore(1);
        }
        else if (isBlueFallen)
        {
            IceSumo_RoundManager.Instance.IncreaseScore(0);
        }
    }
}