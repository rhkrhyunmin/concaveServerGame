using DummyClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Bingo : MonoBehaviour
{

    public static Bingo Instance { get; } = new Bingo();

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
    public TextMeshProUGUI _debugTMP;
    public GameObject cube;

    public BingoExSo _BingoExample;
    public RandomBingo _RandomBingo;

    
    private bool isSpace = false;

    private bool isMyTurn = false;


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
        List<string> bingoPanel = _RandomBingo.selectedItems.OrderBy(x => UnityEngine.Random.value).Take(9).ToList();

        C_RandomIndex movePacketArray = new C_RandomIndex(); // ���ο� ��Ŷ ���� ����

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
                // ������ ��쿡 ���� ó��
            }
        }

        network.Send(movePacketArray.Write());
        progress = GameProgress.Turn;

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
        if (turn == localMark)
        {
            isMyTurn = DoOwnTurn();
            //s_Bingo.Read();
        }

        if (isMyTurn == false)
        {
            // ���� ���� ���� ���Դϴ�.	
            return;
        }
        else
        {
            //��ȣ�� ���̴� ���� ȿ���� ���ϴ�. 
        }

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

        //Debug.Log($"�� ���� :{turn}");


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
                        Debug.Log("�Է��� �̸��� ����Ʈ�� �ֽ��ϴ�.");
                        cube.SetActive(true);

                        // �Էµ� ���ڿ��� ������ ��ȯ�Ͽ� index ������ �����մϴ�.
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


    // �ڽ��� ���� ���� ó��.
    bool DoOwnTurn()
    {
        //_NameInputField.interactable = true;

        Numcheck();

        return true;
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
