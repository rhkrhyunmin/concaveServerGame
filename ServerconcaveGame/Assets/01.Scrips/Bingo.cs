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

    // ���� ���� ��Ȳ.
    private enum GameProgress
    {
        None = 0,       // ���� ���� ��.
        Ready,          // ���� ���� ��ȣ ǥ��.
        Turn,           // ���� ��.
        Result,         // ��� ǥ��.
        GameOver,       // ���� ����.
        Disconnect,     // ���� ����.
    };

    // �� ����.
    private enum Turn
    {
        Own = 0,        // �ڻ��� ��.
        Opponent,       // ����� ��.
    };

    // ��ũ.
    private enum Mark
    {
        Circle = 0,     // ��.
        Cross,          // ��.
    };

    // ���� ���.
    private enum Winner
    {
        None = 0,       // ���� ��.
        Circle,         // �۽¸�.
        Cross,          // ���¸�.
    };



    // ĭ�� ��.
    private const int rowNum = 3;

    // ���� ���� ���� ��ȣǥ�� �ð�.
    private const float waitTime = 1.0f;

    // ��� �ð�.
    private const float turnTime = 100.0f;

    // ��ġ�� ��ȣ�� ����.
    private int[] spaces = new int[rowNum * rowNum];

    // ���� ��Ȳ.
    private GameProgress progress;

    // ������ ��.
    private Mark turn;

    // ���� ��ȣ.
    private Mark localMark;

    // ����Ʈ ��ȣ.
    private Mark remoteMark;

    // ���� �ð�.
    private float timer;

    // ����.
    private Winner winner;

    // ���� ���� �÷���.
    private bool isGameOver;

    // ��� �ð�.
    private float currentTime;

    // ��Ʈ��ũ.
    private NetworkManager network = null;

    // ī����.
    private float step_count = 0.0f;

    //
    // �ؽ�ó ����.
    public Texture circleTexture;
    public Texture crossTexture;


    //
    private static float SPACES_WIDTH = 400.0f;
    private static float SPACES_HEIGHT = 400.0f;

    private static float WINDOW_WIDTH = 640.0f;
    private static float WINDOW_HEIGHT = 480.0f;

    //��� �̸� �Է�
    public TMP_InputField _NameInputField;

    public BingoExSo _BingoExample;
    public RandomBingo _RandomBingo;


    // Use this for initialization
    void Start()
    {
        // Network Ŭ������ ������Ʈ ��������.
        GameObject obj = GameObject.Find("NetworkManager");
        network = obj.GetComponent<NetworkManager>();
        if (network != null)
        {
            network.RegisterEventHandler(EventCallback);
        }

        // ������ �ʱ�ȭ�մϴ�.
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

    // ���� ����, �ܺ� UI���� ȣ����.
    public void GameStart()
    {
        // ���� ���� ���·� �մϴ�.
        progress = GameProgress.Ready;

        // ������ ���� �ϰ� �����մϴ�.
        turn = Mark.Circle;

        // �ڽŰ� ����� ��ȣ�� �����մϴ�.
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
        Debug.Log($"���� ����? : {localMark.ToString()}");

        // ���� ������ Ŭ�����մϴ�.
        isGameOver = false;
    }


    void UpdateReady()
    {
        // ���� ���� ��ȣ ǥ�ø� ��ٸ��ϴ�.
        currentTime += Time.deltaTime;
        //Debug.Log("UpdateReady");

        if (currentTime > waitTime)
        {
            // ���� �����Դϴ�.
            progress = GameProgress.Turn;
        }
    }

    void UpdateTurn()
    {
        //Debug.Log($"�� ���� :{turn}");
        bool setMark = false;
        if (turn == localMark)
        {
            setMark = DoOwnTurn();

            //�� �� ���� ��Ҹ� ������ Ŭ���� ����ȿ���� ���ϴ�.
            if (setMark == false && Input.GetMouseButtonDown(0))
            {

            }
        }
        else
        {
            setMark = DoOppnentTurn();
            //�� �� ���� �� ������ Ŭ���� ���� ȿ���� ���ϴ�.
            
        }

        if (setMark == false)
        {
            // ���� ���� ���� ���Դϴ�.	
            return;
        }
        else
        {
            //��ȣ�� ���̴� ���� ȿ���� ���ϴ�. 
        }

        // ��ȣ�� ������ üũ�մϴ�.
        //winner = CheckInPlacingMarks();
        if (winner != Winner.None)
        {
            //�¸��� ���� ����ȿ���� ���ϴ�.
            if ((winner == Winner.Circle && localMark == Mark.Circle)
                || (winner == Winner.Cross && localMark == Mark.Cross))
            {

            }
            //BGM�������.

            // ���� �����Դϴ�.
            progress = GameProgress.Result;
        }

        // ���� �����մϴ�.

        turn = (turn == Mark.Circle) ? Mark.Cross : Mark.Circle;
        Debug.Log($"�� ���� :{turn}");
        timer = turnTime;
    }

    // ���� ���� ó��
    void UpdateGameOver()
    {
        step_count += Time.deltaTime;
        if (step_count > 1.0f)
        {
            // ������ �����մϴ�.
            Reset();
            isGameOver = true;
        }
    }

    // �ڽ��� ���� ���� ó��.
    bool DoOwnTurn()
    {
        int index = 0;

        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            // Ÿ�ӿ���.
            timer = 0.0f;
            do
            {
                index = UnityEngine.Random.Range(0, 8);
            } while (spaces[index] != -1);
        }
        else
        {
            //
            // ���콺�� ���� ��ư�� ���� ���¸� �����մϴ�.
            bool isClicked = Input.GetKeyDown(KeyCode.Space);
            if (isClicked == true)
            {
                if (_NameInputField != null)
                {
                    Debug.Log(_RandomBingo.selectedItems.Count);
                    if (_RandomBingo.selectedItems != null && _RandomBingo.selectedItems.Count > 0)
                    {
                        string inputName = _NameInputField.text; // ������� �Է� �̸� ��������

                        List<string> selectedItems = _RandomBingo.selectedItems; // ���õ� ������ ����Ʈ ��������

                        if (selectedItems.Contains(inputName))
                        {
                            Debug.Log("�Է��� �̸��� ����Ʈ�� �ֽ��ϴ�.");
                        }
                        else
                        {
                            Debug.Log("�Է��� �̸��� ����Ʈ�� �����ϴ�.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("���õ� �������� ���ų�, selectedItems ����Ʈ�� null�Դϴ�.");
                    }
                }
            }

            if (isClicked == false)
            {
                // �������� �ʾ����Ƿ� �ƹ��͵� ���� ���� �ʽ��ϴ�.
                return false;
            }

            Vector3 pos = Input.mousePosition;
            Debug.Log("POS:" + pos.x + ", " + pos.y + ", " + pos.z);

            // ������ ������ �������� ���õ� ĭ���� ��ȯ�մϴ�.
            index = ConvertPositionToIndex(pos);
            Debug.Log($"Ŭ�� ��ȯ�� : {index}");
            if (index < 0)
            {
                // ���� ���� ���õǾ����ϴ�.
                return false;
            }
        }

        // ĭ�� �Ӵϴ�.
        bool ret = SetMarkToSpace(index, localMark);
        if (ret == false)
        {
            // �� �� �����ϴ�.
            return false;
        }

        // ������ ĭ�� ������ �۽��մϴ�
        C_MoveStone movePacket = new C_MoveStone();
        movePacket.StonePosition = index;
        network.Send(movePacket.Write());

        return true;
    }

    // ����� ���� ���� ó��.
    bool DoOppnentTurn()
    {
        Debug.Log("DoOppnentTurn");

        // ����� ������ �����մϴ�.
        int index = PlayerManager.Instance.returnStone();
        if (index <= 0)
        {
            // ���� ���ŵ��� �ʾҽ��ϴ�.
            Debug.Log($"���ŵ� �� : {index}");
            return false;
        }

        // ������� �� Ŭ���̾�Ʈ��� ���� �����մϴ�.
        Mark mark = (network.IsServer() == true) ? Mark.Cross : Mark.Circle;
        Debug.Log("���ż��ż���");

        // ������ ������ ���õ� ĭ���� ��ȯ�մϴ�. 
        Debug.Log("Recv:" + index + " [" + network.IsServer() + "]");

        // ĭ�� �Ӵϴ�.
        bool ret = SetMarkToSpace(index, remoteMark);
        if (ret == false)
        {
            // �� �� ����.
            Debug.Log("�Ѽ�����.");
            return false;
        }

        return true;


    }

    // 
    int ConvertPositionToIndex(Vector3 pos)
    {
        float sx = SPACES_WIDTH;
        float sy = SPACES_HEIGHT;

        // ���� ���� �� �����̸� �������� �� ��ǥ��� ��ȯ�մϴ�.
        float left = ((float)Screen.width - sx) * 0.5f;
        float top = ((float)Screen.height + sy) * 0.5f;

        float px = pos.x - left;
        float py = top - pos.y;

        if (px < 0.0f || px > sx)
        {
            // �ʵ� ���Դϴ�.
            return -1;
        }

        if (py < 0.0f || py > sy)
        {
            // �ʵ� ���Դϴ�.
            return -1;
        }

        // �ε��� ��ȣ�� ��ȯ�մϴ�.
        float divide = (float)rowNum;
        int hIndex = (int)(px * divide / sx);
        int vIndex = (int)(py * divide / sy);

        int index = vIndex * rowNum + hIndex;

        return index;
    }

    // 
    bool SetMarkToSpace(int index, Mark mark)
    {
        Debug.Log($"�����̽��ε���{spaces[index]}");
        if (spaces[index] == -1)
        {
            // -1�� �̼��õ� ĭ���� ����.
            spaces[index] = (int)mark;
            DrawFieldAndMarks();
            Debug.Log($"�����̽��ε��� ����");
            return true;
        }

        // �̹� ���� �ֽ��ϴ�.
        return false;
    }


    // ���� ����.
    void Reset()
    {
        //turn = Turn.Own;
        turn = Mark.Circle;
        progress = GameProgress.None;

        // �̼������� �ϰ� �ʱ�ȭ�մϴ�.
        for (int i = 0; i < spaces.Length; ++i)
        {
            spaces[i] = -1;
        }
    }


    // �ʵ�� ��ȣ�� �׸��ϴ�.
    void DrawFieldAndMarks()
    {
        float sx = SPACES_WIDTH;
        float sy = SPACES_HEIGHT;
        float left = ((float)Screen.width - sx) * 0.5f;
        float top = ((float)Screen.height - sy) * 0.5f;

        // ��ȣ�� �׸��ϴ�. 
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
                Debug.Log($"���׸��� : {texture}");

                float ofs = sx / divide * 0.1f;
                Graphics.DrawTexture(new Rect(px + ofs, py + ofs, sx * 0.8f / divide, sy * 0.8f / divide), texture);

            }
        }

        // ���� �ؽ�ó ǥ��.
        /*
        if (localMark == turn)
        {
            float offset = (localMark == Mark.Circle) ? -94.0f : sx + 36.0f;
            rect = new Rect(left + offset, top + 5.0f, 68.0f, 136.0f);
            Graphics.DrawTexture(rect, youTexture);
        }
        */
    }


    // ���� ����.
    void NotifyDisconnection()
    {
        string message = "ȸ���� ������ϴ�.\n\n��ư�� ��������.";

    }



    // ���� ���� üũ.
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // �̺�Ʈ �߻� ���� �ݹ� �Լ�.
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
