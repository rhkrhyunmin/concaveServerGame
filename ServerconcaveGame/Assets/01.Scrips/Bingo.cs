using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DummyClient;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class Bingo : MonoBehaviour
{

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

    public BingoExSo _BingoExample;
    public RandomBingo _RandomBingo;


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
        // 시합 시작 신호 표시를 기다립니다.
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
        //Debug.Log($"턴 시작 :{turn}");
        bool setMark = false;
        if (turn == localMark)
        {
            setMark = DoOwnTurn();

            //둘 수 없는 장소를 누르면 클릭용 사운드효과를 냅니다.
            if (setMark == false && Input.GetMouseButtonDown(0))
            {

            }
        }
        else
        {
            setMark = DoOppnentTurn();
            //둘 수 없을 때 누르면 클릭용 사운드 효과를 냅니다.
            
        }

        if (setMark == false)
        {
            // 놓을 곳을 검토 중입니다.	
            return;
        }
        else
        {
            //기호가 놓이는 사운드 효과를 냅니다. 
        }

        // 기호의 나열을 체크합니다.
        //winner = CheckInPlacingMarks();
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

        turn = (turn == Mark.Circle) ? Mark.Cross : Mark.Circle;
        Debug.Log($"턴 갱신 :{turn}");
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

    // 자신의 턴일 때의 처리.
    bool DoOwnTurn()
    {
        int index = 0;

        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            // 타임오버.
            timer = 0.0f;
            do
            {
                index = UnityEngine.Random.Range(0, 8);
            } while (spaces[index] != -1);
        }
        else
        {
            //
            // 마우스의 왼쪽 버튼의 눌린 상태를 감시합니다.
            bool isClicked = Input.GetKeyDown(KeyCode.Space);
            if (isClicked == true)
            {
                if (_NameInputField != null)
                {
                    Debug.Log(_RandomBingo.selectedItems.Count);
                    if (_RandomBingo.selectedItems != null && _RandomBingo.selectedItems.Count > 0)
                    {
                        string inputName = _NameInputField.text; // 사용자의 입력 이름 가져오기

                        List<string> selectedItems = _RandomBingo.selectedItems; // 선택된 아이템 리스트 가져오기

                        if (selectedItems.Contains(inputName))
                        {
                            Debug.Log("입력한 이름이 리스트에 있습니다.");
                        }
                        else
                        {
                            Debug.Log("입력한 이름이 리스트에 없습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("선택된 아이템이 없거나, selectedItems 리스트가 null입니다.");
                    }
                }
            }

            if (isClicked == false)
            {
                // 눌려지지 않았으므로 아무것도 하지 않지 않습니다.
                return false;
            }

            Vector3 pos = Input.mousePosition;
            Debug.Log("POS:" + pos.x + ", " + pos.y + ", " + pos.z);

            // 수신한 정보를 바탕으로 선택된 칸으로 변환합니다.
            index = ConvertPositionToIndex(pos);
            Debug.Log($"클릭 변환값 : {index}");
            if (index < 0)
            {
                // 범위 밖이 선택되었습니다.
                return false;
            }
        }

        // 칸에 둡니다.
        bool ret = SetMarkToSpace(index, localMark);
        if (ret == false)
        {
            // 둘 수 없습니다.
            return false;
        }

        // 선택한 칸의 정보를 송신합니다
        C_MoveStone movePacket = new C_MoveStone();
        movePacket.StonePosition = index;
        network.Send(movePacket.Write());

        return true;
    }

    // 상대의 턴일 때의 처리.
    bool DoOppnentTurn()
    {
        Debug.Log("DoOppnentTurn");

        // 상대의 정보를 수신합니다.
        int index = PlayerManager.Instance.returnStone();
        if (index <= 0)
        {
            // 아직 수신되지 않았습니다.
            Debug.Log($"수신된 값 : {index}");
            return false;
        }

        // 서버라면 ○ 클라이언트라면 ×를 지정합니다.
        Mark mark = (network.IsServer() == true) ? Mark.Cross : Mark.Circle;
        Debug.Log("수신수신수신");

        // 수신한 정보를 선택된 칸으로 변환합니다. 
        Debug.Log("Recv:" + index + " [" + network.IsServer() + "]");

        // 칸에 둡니다.
        bool ret = SetMarkToSpace(index, remoteMark);
        if (ret == false)
        {
            // 둘 수 없다.
            Debug.Log("둘수없다.");
            return false;
        }

        return true;


    }

    // 
    int ConvertPositionToIndex(Vector3 pos)
    {
        float sx = SPACES_WIDTH;
        float sy = SPACES_HEIGHT;

        // 맆드 왼쪽 위 모퉁이를 기점으로 한 좌표계로 변환합니다.
        float left = ((float)Screen.width - sx) * 0.5f;
        float top = ((float)Screen.height + sy) * 0.5f;

        float px = pos.x - left;
        float py = top - pos.y;

        if (px < 0.0f || px > sx)
        {
            // 필드 밖입니다.
            return -1;
        }

        if (py < 0.0f || py > sy)
        {
            // 필드 밖입니다.
            return -1;
        }

        // 인덱스 번호로 변환합니다.
        float divide = (float)rowNum;
        int hIndex = (int)(px * divide / sx);
        int vIndex = (int)(py * divide / sy);

        int index = vIndex * rowNum + hIndex;

        return index;
    }

    // 
    bool SetMarkToSpace(int index, Mark mark)
    {
        Debug.Log($"스페이스인덱스{spaces[index]}");
        if (spaces[index] == -1)
        {
            // -1은 미선택된 칸임을 뜻함.
            spaces[index] = (int)mark;
            DrawFieldAndMarks();
            Debug.Log($"스페이스인덱스 변경");
            return true;
        }

        // 이미 놓여 있습니다.
        return false;
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


    // 필드와 기호를 그립니다.
    void DrawFieldAndMarks()
    {
        float sx = SPACES_WIDTH;
        float sy = SPACES_HEIGHT;
        float left = ((float)Screen.width - sx) * 0.5f;
        float top = ((float)Screen.height - sy) * 0.5f;

        // 기호를 그립니다. 
        for (int index = 0; index < spaces.Length; ++index)
        {
            if (spaces[index] != -1)
            {
                int x = index % rowNum;
                int y = index / rowNum;

                float divide = (float)rowNum;
                float px = left + x * sx / divide;
                float py = top + y * sy / divide;

                Texture texture = (spaces[index] == 0) ? circleTexture : crossTexture;
                Debug.Log($"돌그리기 : {texture}");

                float ofs = sx / divide * 0.1f;
                Graphics.DrawTexture(new Rect(px + ofs, py + ofs, sx * 0.8f / divide, sy * 0.8f / divide), texture);

            }
        }

        // 순서 텍스처 표시.
        /*
        if (localMark == turn)
        {
            float offset = (localMark == Mark.Circle) ? -94.0f : sx + 36.0f;
            rect = new Rect(left + offset, top + 5.0f, 68.0f, 136.0f);
            Graphics.DrawTexture(rect, youTexture);
        }
        */
    }


    // 끊김 통지.
    void NotifyDisconnection()
    {
        string message = "회선이 끊겼습니다.\n\n버튼을 누르세요.";

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
