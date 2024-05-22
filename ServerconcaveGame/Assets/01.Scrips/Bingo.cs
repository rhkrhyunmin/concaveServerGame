using DummyClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Bingo : MonoBehaviour
{

    public static Bingo Instance { get; } = new Bingo();

    // 게임 진행 상황.
    private enum GameProgress
    {
        None = 0,       // 시합 시작 전.
        Ready,          // 시합 시작 신호 표시.
        Turn,           // 시함 중.
        Result,         // 결과 표시.
        GameOver,       // 게임 종료.
        Disconnect,     // 연결 끊기.
    };

    // 턴 종류.
    private enum Turn
    {
        Own = 0,        // 자산의 턴.
        Opponent,       // 상대의 턴.
    };

    // 마크.
    private enum Mark
    {
        Circle = 0,     // ○.
        Cross,          // ×.
    };

    // 시합 결과.
    private enum Winner
    {
        None = 0,       // 시합 중.
        Circle,         // ○승리.
        Cross,          // ×승리.
    };



    // 칸의 수.
    private const int rowNum = 3;

    // 시합 시작 전의 신호표시 시간.
    private const float waitTime = 1.0f;

    // 대기 시간.
    private const float turnTime = 100.0f;

    // 배치된 기호를 보존.
    private int[] spaces = new int[rowNum * rowNum];

    // 진행 상황.
    private GameProgress progress;

    // 현재의 턴.
    private Mark turn;

    // 로컬 기호.
    private Mark localMark;

    // 리모트 기호.
    private Mark remoteMark;

    // 남은 시간.
    private float timer;

    // 승자.
    private Winner winner;

    // 게임 종료 플래그.
    private bool isGameOver;

    // 대기 시간.
    private float currentTime;

    // 네트워크.
    private NetworkManager network = null;

    // 카운터.
    private float step_count = 0.0f;

    //
    // 텍스처 관련.
    public Texture circleTexture;
    public Texture crossTexture;


    //
    private static float SPACES_WIDTH = 400.0f;
    private static float SPACES_HEIGHT = 400.0f;

    private static float WINDOW_WIDTH = 640.0f;
    private static float WINDOW_HEIGHT = 480.0f;

    //사람 이름 입력
    public TMP_InputField _NameInputField;
    public TextMeshProUGUI _debugTMP;
    public GameObject cube;

    public BingoExSo _BingoExample;
    public RandomBingo _RandomBingo;

    
    private bool isSpace = false;

    private bool isMyTurn = false;


    // Use this for initialization
    void Start()
    {
        // Network 클래스의 컴포넌트 가져오기.
        GameObject obj = GameObject.Find("NetworkManager");
        network = obj.GetComponent<NetworkManager>();
        if (network != null)
        {
            network.RegisterEventHandler(EventCallback);
        }

        // 게임을 초기화합니다.
        Reset();
        isGameOver = false;
        timer = turnTime;
    }

    // Update is called once per frame
    void Update()
    {
        switch (progress)
        {
            case GameProgress.Ready:
                UpdateReady();
                break;

            case GameProgress.Turn:
                UpdateTurn();
                break;

            case GameProgress.GameOver:
                UpdateGameOver();
                break;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            turn = (turn == Mark.Circle) ? Mark.Cross : Mark.Circle;
            isSpace = true;
        }
        else
        {
            isSpace = false;
        }
       


        GameManager.Instance.inputText = _NameInputField.text;
    }

    // 게임 시작, 외부 UI에서 호출함.
    public void GameStart()
    {
        // 게임 시작 상태로 합니다.
        progress = GameProgress.Ready;

        // 서버가 먼저 하게 설정합니다.
        turn = Mark.Circle;

        // 자신과 상대의 기호를 설정합니다.
        if (network.IsServer() == true)
        {
            localMark = Mark.Circle;
            remoteMark = Mark.Cross;
        }
        else
        {
            localMark = Mark.Cross;
            remoteMark = Mark.Circle;
        }
        Debug.Log($"나는 누구? : {localMark.ToString()}");

        // 이전 설정을 클리어합니다.
        isGameOver = false;
    }


    void UpdateReady()
    {
        List<string> bingoPanel = _RandomBingo.selectedItems.OrderBy(x => UnityEngine.Random.value).Take(9).ToList();

        C_RandomIndex movePacketArray = new C_RandomIndex(); // 새로운 패킷 형식 정의

        foreach (string item in bingoPanel)
        {
            int parsedValue;
            if (int.TryParse(item, out parsedValue))
            {
                movePacketArray.values.Add(parsedValue);
                Debug.Log(parsedValue);   
            }
            else
            {
                // 실패한 경우에 대한 처리
            }
        }

        network.Send(movePacketArray.Write());
        progress = GameProgress.Turn;

        currentTime += Time.deltaTime;
        //Debug.Log("UpdateReady");

        if (currentTime > waitTime)
        {
            // 게임 시작입니다.
            progress = GameProgress.Turn;
        }
    }

    void UpdateTurn()
    {
        if (turn == localMark)
        {
            isMyTurn = DoOwnTurn();
            //s_Bingo.Read();
        }

        if (isMyTurn == false)
        {
            // 놓을 곳을 검토 중입니다.	
            return;
        }
        else
        {
            //기호가 놓이는 사운드 효과를 냅니다. 
        }

        if (winner != Winner.None)
        {
            //승리한 경우는 사운드효과를 냅니다.
            if ((winner == Winner.Circle && localMark == Mark.Circle)
                || (winner == Winner.Cross && localMark == Mark.Cross))
            {

            }
            //BGM재생종료.

            // 게임 종료입니다.
            progress = GameProgress.Result;
        }

        // 턴을 갱신합니다.

        //Debug.Log($"턴 갱신 :{turn}");


        timer = turnTime;
    }

    // 게임 종료 처리
    void UpdateGameOver()
    {
        step_count += Time.deltaTime;
        if (step_count > 1.0f)
        {
            // 게임을 종료합니다.
            Reset();
            isGameOver = true;
        }
    }

    public bool Numcheck()
    {
        if (isSpace == true)
        {
            if (_NameInputField != null)
            {
                if (_RandomBingo.selectedItems != null && _RandomBingo.selectedItems.Count > 0)
                {
                    List<string> selectedItems = _RandomBingo.selectedItems;
                    if (selectedItems.Contains(GameManager.Instance.inputText))
                    {
                        Debug.Log("입력한 이름이 리스트에 있습니다.");
                        cube.SetActive(true);

                        // 입력된 문자열을 정수로 변환하여 index 변수에 저장합니다.
                        if (int.TryParse(GameManager.Instance.inputText, out int index))
                        {
                            C_Bingo movePacket = new C_Bingo();
                            int bingoValue;
                            if (int.TryParse(GameManager.Instance.inputText, out bingoValue))
                            {
                                movePacket.c_bingo = bingoValue;
                                network.Send(movePacket.Write());
                            }
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    // 자신의 턴일 때의 처리.
    bool DoOwnTurn()
    {
        //_NameInputField.interactable = true;

        Numcheck();

        return true;
    }

    // 게임 리셋.
    void Reset()
    {
        //turn = Turn.Own;
        turn = Mark.Circle;
        progress = GameProgress.None;

        // 미선택으로 하고 초기화합니다.
        for (int i = 0; i < spaces.Length; ++i)
        {
            spaces[i] = -1;
        }
    }

    // 게임 종료 체크.
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // 이벤트 발생 시의 콜백 함수.
    public void EventCallback(NetEventState state)
    {
        switch (state.type)
        {
            case NetEventType.Disconnect:
                if (progress < GameProgress.Result && isGameOver == false)
                {
                    progress = GameProgress.Disconnect;
                }
                break;
        }
    }
}
