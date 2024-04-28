using DummyClient;
using DummyClient.Session;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{   // DummyClient ����

    // ���� �÷���.	
    public bool m_isServer = false;
    // ���� �÷���.
    private bool m_isConnected = false;

    // �̺�Ʈ �˸� ��������Ʈ.
    public delegate void EventHandler(NetEventState state);
    private EventHandler m_handler;

    // �̺�Ʈ �Լ� ���.
    public void RegisterEventHandler(EventHandler handler)
    {
        m_handler += handler;
    }

    // �̺�Ʈ ���� �Լ� ����.
    public void UnregisterEventHandler(EventHandler handler)
    {
        m_handler -= handler;
    }

    // ���� 1���� ��� �����̹Ƿ�, SessionManager �̻��
    ServerSession _session = new ServerSession();


    // �������� ������ ����!
    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
        Debug.Log("보냄");
    }

    public bool ConnectToServer(IPAddress ipAddr, int port)
    {
        try
        {
            Debug.Log("���� ����.");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port); // IP�ּ�, ��Ʈ��ȣ �Է�
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return _session; }, 1);
            Debug.Log("Connection success.");
            m_isConnected = true;
        }
        catch (Exception e)
        {
            Debug.Log("Connect fail");
            return false;
        }

        if (m_handler != null)
        {
            // ���� ����� �˸��ϴ�.
            NetEventState state = new NetEventState();
            state.type = NetEventType.Connect;
            state.result = (m_isConnected == true) ? NetEventResult.Success : NetEventResult.Failure;
            m_handler(state);
            Debug.Log("event handler called");

        }

        return true;
    }


    // �������� Ȯ��.
    public bool IsServer()
    {
        return m_isServer;
    }

    // ���� Ȯ��.
    public bool IsConnected()
    {
        return m_isConnected;
    }




    void Start()
    {

    }
    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
            PacketManager.Instance.HandlePacket(_session, packet);
    }
}
