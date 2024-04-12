using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class SequenceManager : MonoBehaviour
{
    enum Mode
    {
        SelectHost = 0,
        Connection,
        Game,
        Disconnection,
        Error,
    };

    enum HostType
    {
        None = 0,
        Server,
        Client,
    };

    private Mode m_mode;
    private IPAddress ipAddr;
    private HostType hostType;
    private const int m_port = 50765;
    private int m_counter = 0;

    public GameObject UI_MainMenu;
    public GameObject UI_Game;
    public NetworkManager network;

    public void OnClick_BtnMakeRoom()
    {
        hostType = HostType.Server;

        UI_MainMenu.SetActive(false);
        UI_Game.SetActive(true);
    }

    public void OnClick_BtnEnterRoom()
    {
        hostType = HostType.Client;

        UI_MainMenu.SetActive(false);
        UI_Game.SetActive(true);
    }
    public void OnClick_Exit()
    {
        hostType = HostType.None;
        m_mode = Mode.Disconnection;

        UI_MainMenu.SetActive(true);
        UI_Game.SetActive(false);
    }


    private void Awake()
    {
        m_mode = Mode.SelectHost;
        hostType = HostType.None;

        //GameObject obj = new GameObject("Network");
        //network = obj.AddComponent<NetworkManager>();
        //DontDestroyOnLoad(obj);

        // ȣ��Ʈ���� �����ɴϴ�.
        string hostname = Dns.GetHostName();
        // ȣ��Ʈ������ IP�ּҸ� �����ɴϴ�.
        IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
        ipAddr = iphost.AddressList[1];
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (m_mode)
        {
            case Mode.SelectHost:
                OnUpdateSelectHost();
                break;

            case Mode.Connection:
                OnUpdateConnection();
                break;

            case Mode.Game:
                OnUpdateGame();
                break;

            case Mode.Disconnection:
                //OnUpdateDisconnection();
                break;
        }

        ++m_counter;
    }

    // Sever �Ǵ� Client ����ȭ��
    void OnUpdateSelectHost()
    {
        switch (hostType)
        {
            case HostType.Server:
                {
                    bool ret = network.ConnectToServer(ipAddr, m_port);
                    m_mode = ret ? Mode.Connection : Mode.Error;
                    Debug.Log("HostType Server");
                    network.m_isServer = true;
                }
                break;

            case HostType.Client:
                {
                    bool ret = network.ConnectToServer(ipAddr, m_port);
                    m_mode = ret ? Mode.Connection : Mode.Error;
                    Debug.Log("HostType Client");

                }
                break;

            default:
                break;
        }
    }

    void OnUpdateConnection()
    {
        if (network.IsConnected() == true)
        {
            m_mode = Mode.Game;

            GameObject game = GameObject.Find("Bingo");
            game.GetComponent<Bingo>().GameStart();
        }
    }

    void OnUpdateGame()
    {
        GameObject game = GameObject.Find("Bingo");
        if (game.GetComponent<Bingo>().IsGameOver() == true)
        {
            m_mode = Mode.Disconnection;
        }
    }



}

