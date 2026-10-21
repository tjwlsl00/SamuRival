using UnityEngine;
using System.Collections.Generic;

public class Archering_DeadZone : MonoBehaviour
{
    // 싱글톤
    public static Archering_DeadZone Instance;

    // 본인 트리거 안 오브젝트 목록
    private List<GameObject> insideStones = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #region 접촉 오브젝트 확인
    private void OnTriggerEnter(Collider other)
    {
        if (!insideStones.Contains(other.gameObject))
        {
            insideStones.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (insideStones.Contains(other.gameObject))
        {
            insideStones.Remove(other.gameObject);
        }
    }
    #endregion

    // 데드존 위 오브젝트 삭제
    public void DeleteStone()
    {
        if (Archering_TurnManager.Instance.gameTurn == Archering_TurnManager.GameTurn.RedTurn)
        {   
            foreach (GameObject insideStone in insideStones)
            {
                if (insideStone != null && insideStone.CompareTag("blueStone"))
                {
                    Debug.Log("데드존 블루 스톤 삭제");

                    Destroy(insideStone);
                }
            }
        }
        else
        {
            foreach (GameObject insideStone in insideStones)
            {
                if (insideStone != null && insideStone.CompareTag("redStone"))
                {
                    Debug.Log("데드존 레드 스톤 삭제");

                    Destroy(insideStone);
                }
            }
        }
    }
}