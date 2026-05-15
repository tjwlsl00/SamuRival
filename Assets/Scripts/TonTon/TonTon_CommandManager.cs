using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TonTon_CommandManager : MonoBehaviour
{
    // 커맨드 종류
    public enum CommandType { Up, Down, Left, Right }

    // 화면에 뜰 커맨드 
    public List<CommandType> redLine = new List<CommandType>();
    public List<CommandType> blueLine = new List<CommandType>();

    // UI
    [SerializeField] GameObject[] Red_commandVisuals;
    [SerializeField] GameObject[] Blue_commandVisuals;
    [SerializeField] Sprite[] Red_commandIcons;
    [SerializeField] Sprite[] Blue_commandIcons;

    // 한줄에 표시 될 개수
    private int commandsPerLine = 5;
    private int redIndex = 0;
    private int blueIndex = 0;

    // 콤보 수치
    public int BlueCurrentCombo = 0;
    public int RedCurrentCombo = 0;
    public int maxCombo = 5;

    // 집 오브젝트 
    [SerializeField] GameObject[] playerBuilding;
    private BuildObj redBuildObj;
    private BuildObj blueBuildObj;

    // 플레이어 
    public GameObject[] TonTon_Players;
    private TonTon_Player tonton_Red;
    private TonTon_Player tonton_Blue;

    // 외부 
    private TonTon_UIManager tonTon_UIManager;
    private TonTon_SoundManger tonTon_SoundManger;

    void Awake()
    {
        tonton_Red = TonTon_Players[0].GetComponent<TonTon_Player>();
        tonton_Blue = TonTon_Players[1].GetComponent<TonTon_Player>();

        // 외부 
        tonTon_UIManager = GetComponent<TonTon_UIManager>();
        tonTon_SoundManger = GetComponent<TonTon_SoundManger>();
        redBuildObj = playerBuilding[0].GetComponent<BuildObj>();
        blueBuildObj = playerBuilding[1].GetComponent<BuildObj>();
    }

    void Start()
    {
        GenerateInitialCommands(TonTon_TeamSide.Red);
        GenerateInitialCommands(TonTon_TeamSide.Blue);
    }

    void Update()
    {
        // 게임 끝나면 키 입력 무시 
        if (TonTon_GameManager.Instance.tonton_GameDirection == TonTon_GameManager.TonTon_GameDirection.End) return;

        // 커맨드 키 입력
        HandleInput();
    }

    void HandleInput()
    {
        // 레드(언 상태가 아닐때만)
        if (!tonTon_UIManager.isRedFrost)
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CheckCommand(tonton_Red, CommandType.Up);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CheckCommand(tonton_Red, CommandType.Down);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CheckCommand(tonton_Red, CommandType.Left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CheckCommand(tonton_Red, CommandType.Right);
            }

        // 블루
        if (!tonTon_UIManager.isBlueFrost)
            if (Input.GetKeyDown(KeyCode.W))
            {
                CheckCommand(tonton_Blue, CommandType.Up);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                CheckCommand(tonton_Blue, CommandType.Down);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                CheckCommand(tonton_Blue, CommandType.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                CheckCommand(tonton_Blue, CommandType.Right);
            }
    }

    #region 커맨드 초기화, 랜덤 생성
    void GenerateInitialCommands(TonTon_TeamSide side)
    {
        if (side == TonTon_TeamSide.Red)
        {
            redLine.Clear();
            redIndex = 0;

            for (int i = 0; i < commandsPerLine; i++)
            {
                CommandType randomCmd = (CommandType)Random.Range(0, 4);
                redLine.Add(randomCmd);
            }
        }
        else
        {
            blueLine.Clear();
            blueIndex = 0;

            for (int i = 0; i < commandsPerLine; i++)
            {
                CommandType randomCmd = (CommandType)Random.Range(0, 4);
                blueLine.Add(randomCmd);
            }
        }

        UpdateCommandUI(side);
    }
    #endregion

    #region 커맨크 체크, 한 줄 성공
    void CheckCommand(TonTon_Player player, CommandType input)
    {
        if (player.tonton_TeamSide == TonTon_TeamSide.Red)
        {
            if (redLine[redIndex] == input)
            {
                // 다음 칸으로 이동
                redIndex++;

                // 맞춘 아이콘 색상 변경
                Red_commandVisuals[redIndex - 1].GetComponent<Image>().color = Color.gray;

                // 오디오
                tonton_Red.PlayCommandClip();

                // 한줄 모두 맞추면
                if (redIndex >= commandsPerLine)
                {
                    OnLineComplete(TonTon_TeamSide.Red);
                }
            }
            else
            {
                // 처음부터 다시 시작
                redIndex = 0;
                UpdateCommandUI(TonTon_TeamSide.Red);

                Debug.Log("틀림! 처음부터 다시");

                // 틀림 패널티
                float penalty = 0f;
                float currentGaze = TonTon_GameManager.Instance.CurrentRedGaze;

                if (currentGaze <= 40f)
                {
                    penalty = 8f;
                }
                else if (currentGaze <= 80f)
                {
                    penalty = 5f;
                }
                else
                {
                    penalty = 2f;
                }

                // 게이지 감소 및 최소값(0) 고정
                TonTon_GameManager.Instance.CurrentRedGaze = Mathf.Max(0f, currentGaze - penalty);

                // 콤보값 초기화
                RedCurrentCombo = 0;
                Debug.Log("레드 콤보 초기화");

                // 집 오브젝트 투명도 조절
                redBuildObj.SetGaugeStep(TonTon_GameManager.Instance.CurrentRedGaze);

                // 변경된 수치 UI 업데이트 
                tonTon_UIManager.UpdatePlayerBuildGazeUI(0);

                // 커맨드 패널 흔들기 
                tonTon_UIManager.ShakeCommandPanel(0);
            }
        }
        else
        {
            if (blueLine[blueIndex] == input)
            {
                // 다음 칸으로 이동
                blueIndex++;

                // 맞춘 아이콘 색상 변경
                Blue_commandVisuals[blueIndex - 1].GetComponent<Image>().color = Color.gray;

                // 오디오
                tonton_Blue.PlayCommandClip();

                // 한줄 모두 맞추면
                if (blueIndex >= commandsPerLine)
                {
                    OnLineComplete(TonTon_TeamSide.Blue);
                }
            }
            else
            {
                // 처음부터 다시 시작
                blueIndex = 0;
                UpdateCommandUI(TonTon_TeamSide.Blue);

                Debug.Log("틀림! 처음부터 다시");

                float penalty = 0f;
                float currentGaze = TonTon_GameManager.Instance.CurrentBlueGaze;

                // 틀림 패널티
                if (currentGaze <= 40f)
                {
                    penalty = 8f;
                }
                else if (currentGaze <= 80f)
                {
                    penalty = 5f;
                }
                else
                {
                    penalty = 2f;
                }

                // 게이지 감소 및 최소값(0) 고정
                TonTon_GameManager.Instance.CurrentBlueGaze = Mathf.Max(0f, currentGaze - penalty);

                // 콤보값 초기화
                BlueCurrentCombo = 0;
                Debug.Log("블루 콤보 초기화");

                // 집 오브젝트 투명도 조절
                blueBuildObj.SetGaugeStep(TonTon_GameManager.Instance.CurrentBlueGaze);

                // 변경된 수치 UI 업데이트 
                tonTon_UIManager.UpdatePlayerBuildGazeUI(1);

                // 커맨드 패널 흔들기 
                tonTon_UIManager.ShakeCommandPanel(1);
            }
        }
    }

    void OnLineComplete(TonTon_TeamSide side)
    {
        Debug.Log(side + " 한 줄 성공!");

        if (side == TonTon_TeamSide.Red)
        {
            Invoke("RegenRed", 0.1f);

            // 게이지 증가 
            float currentGaze = TonTon_GameManager.Instance.CurrentRedGaze;
            float addValue = 0f;

            if (currentGaze <= 40f)
            {
                addValue = 8f;
            }
            else if (currentGaze <= 80f)
            {
                addValue = 5f;
            }
            else
            {
                addValue = 2f;
            }

            TonTon_GameManager.Instance.CurrentRedGaze += addValue;

            // 콤보 증가 
            IncreaseCombo(0);

            // 플레이어 이동
            tonton_Red.MoveToWayPoint();

            // UI 업데이트 
            tonTon_UIManager.UpdatePlayerBuildGazeUI(0);

            // 커맨드 패널 작아졌다 커짐
            tonTon_UIManager.SucessCommandPanel(0);

            // 해머UI
            tonTon_UIManager.VisibleHammerUI(0);

            // 집 오브젝트 흔드림 효과 
            redBuildObj.ShackBuildObj();

            // 집 오브젝트 투명도 조절
            redBuildObj.SetGaugeStep(TonTon_GameManager.Instance.CurrentRedGaze);

            // 오디오
            tonTon_SoundManger.PlayHammerClip();
        }
        else
        {
            Invoke("RegenBlue", 0.1f);

            float currentGaze = TonTon_GameManager.Instance.CurrentBlueGaze;
            float addValue = 0f;

            // 게이지 증가 
            if (currentGaze <= 40f)
            {
                addValue = 8f;
            }
            else if (currentGaze <= 80f)
            {
                addValue = 5f;
            }
            else
            {
                addValue = 2f;
            }

            TonTon_GameManager.Instance.CurrentBlueGaze += addValue;

            // 콤보 증가 
            IncreaseCombo(1);

            // 플레이어 이동
            tonton_Blue.MoveToWayPoint();

            // UI 업데이트 
            tonTon_UIManager.UpdatePlayerBuildGazeUI(1);

            // 커맨드 패널 작아졌다 커짐
            tonTon_UIManager.SucessCommandPanel(1);

            // 해머UI
            tonTon_UIManager.VisibleHammerUI(1);

            // 집 오브젝트 흔드림 효과 
            blueBuildObj.ShackBuildObj();

            // 집 오브젝트 투명도 조절
            blueBuildObj.SetGaugeStep(TonTon_GameManager.Instance.CurrentBlueGaze);

            // 오디오
            tonTon_SoundManger.PlayHammerClip();
        }
    }

    // 딜레이 이후 새로운 줄 생성
    void RegenRed() => GenerateInitialCommands(TonTon_TeamSide.Red);
    void RegenBlue() => GenerateInitialCommands(TonTon_TeamSide.Blue);
    #endregion

    #region 업데이트 커맨드 UI(순번에 맞는거 자동 할당)
    void UpdateCommandUI(TonTon_TeamSide side)
    {
        if (side == TonTon_TeamSide.Red)
        {
            for (int i = 0; i < redLine.Count; i++)
            {
                // 리스트에 맞는 아이콘 세팅
                Image image = Red_commandVisuals[i].GetComponent<Image>();

                //i번째 커맨드가 무엇인지 가져와서 숫자로 바꿉니다
                int spriteIndex = (int)redLine[i];

                image.sprite = Red_commandIcons[spriteIndex];

                // 색상 초기화(틀렸거나 새로 시작할 때)
                image.color = Color.white;
            }
        }
        else
        {
            for (int i = 0; i < blueLine.Count; i++)
            {
                // 리스트에 맞는 아이콘 세팅
                Image image = Blue_commandVisuals[i].GetComponent<Image>();

                //i번째 커맨드가 무엇인지 가져와서 숫자로 바꿉니다
                int spriteIndex = (int)blueLine[i];

                image.sprite = Blue_commandIcons[spriteIndex];

                // 색상 초기화(틀렸거나 새로 시작할 때)
                image.color = Color.white;
            }
        }
    }
    #endregion

    #region 플레이어 콤보
    private void IncreaseCombo(int PlayerNum)
    {
        if (PlayerNum == 0)
        {
            RedCurrentCombo++;
        }
        else
        {
            BlueCurrentCombo++;
        }

        // 콤보 패널 업데이트 
        tonTon_UIManager.ShowComboPanel(PlayerNum);
    }

    public void ResetPlayerCombo(int PlayerNum)
    {
        Debug.Log("맥스이므로 다시 0으로 초기화 합니다.");

        if (PlayerNum == 0)
        {
            RedCurrentCombo = 0;
        }
        else
        {
            BlueCurrentCombo = 0;
        }
    }
    #endregion
}