using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("스톤 참조")]
    [SerializeField] GameObject decideRedStone;
    [SerializeField] GameObject decideBlueStone;
    [SerializeField] GameObject[] redStones;
    [SerializeField] GameObject[] blueStones;

    [Header("UI 레퍼런스 연결")]
    [SerializeField] private RectTransform redArrow;
    [SerializeField] private Slider redSlider;
    [SerializeField] private RectTransform blueArrow;
    [SerializeField] private Slider blueSlider;

    private Transform stoneSpawnPoint;

    // 외부 
    [SerializeField] GameObject uiManager;
    private Archering_UIManager archering_UIManager;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;    
        }
        else
        {
            Destroy(gameObject);
        }

        stoneSpawnPoint = this.transform;

        // 스크립트 참조 
        archering_UIManager = uiManager.GetComponent<Archering_UIManager>();
    }

    #region 스톤 세팅(디사이드 / 턴)
    public void SetUpDecideStone(int playerNum)
    {
        GameObject targetDicideStone = (playerNum == 0) ? decideRedStone : decideBlueStone;

        if (targetDicideStone != null)
        {
            GameObject spawnedStone = Instantiate(targetDicideStone, stoneSpawnPoint.position, stoneSpawnPoint.rotation);

            Stone stoneScript = spawnedStone.GetComponent<Stone>();

            // UI 연결
            if (stoneScript != null)
            {
                if (playerNum == 0)
                {
                    stoneScript.InitUI(redArrow, redSlider);
                }
                else
                {
                    stoneScript.InitUI(blueArrow, blueSlider);
                }
            }

            // 조작 가이드 UI처리 
            archering_UIManager.ActiveGuidePanel();
        }
    }

    public void SetUpStone(int playerNum, int stoneIndex)
    {
        int index = stoneIndex - 1;

        GameObject[] targetStones = (playerNum == 0) ? redStones : blueStones;

        if (targetStones != null && index >= 0 && index < targetStones.Length)
        {
            if (targetStones[index] != null)
            {
                GameObject spawnedStone = Instantiate(targetStones[index], stoneSpawnPoint.position, stoneSpawnPoint.rotation);

                Stone stoneScript = spawnedStone.GetComponent<Stone>();

                // UI 연결
                if (stoneScript != null)
                {
                    if (playerNum == 0)
                    {
                        stoneScript.InitUI(redArrow, redSlider);
                    }
                    else
                    {
                        stoneScript.InitUI(blueArrow, blueSlider);
                    }
                }

                // 조작 가이드
                archering_UIManager.ActiveGuidePanel();

                // CCTV
                archering_UIManager.VisibleCCTVPanel(playerNum);
            }
        }
    }
    #endregion


}
