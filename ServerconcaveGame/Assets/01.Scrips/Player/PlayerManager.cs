using DummyClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static DummyClient.S_PlayerList;

public class PlayerManager
{
    public TextMeshProUGUI DebugTMP;

    Player _myPlayer;
    Bingo _bingo;
    int StonePosition;


    // ���ӵ��ִ� �÷��̾���� ���
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();


    // �� ���� ����
    public void BroadCastStone(S_BroadcastEnterGame packet)
    {
        StonePosition = packet.playerId;
        Debug.Log($"���������� ���� : {StonePosition}");
    }

    public int returnStone()
    {
        return StonePosition;
    }


    // �÷��̾� ����Ʈ ����&����
    public void Add(S_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach (S_PlayerList.Player p in packet.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;

            if (p.isSelf)
            {
                MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.PlayerId = p.playerId;
                myPlayer.transform.position = new Vector3(-8, 5, 0);
                _myPlayer = myPlayer;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.PlayerId = p.playerId;
                player.transform.position = new Vector3(8, -5, 0);
                _players.Add(p.playerId, player);
            }
        }
    }

    // �� Ȥ�� �������� ���� �������� ��
    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _myPlayer.PlayerId)
            return;

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        Player player = go.AddComponent<Player>();
        _players.Add(packet.playerId, player);
    }

    public void GiveBingoValue(C_Bingo c_Bingo)
    {
       /*if (c_Bingo.c_bingo == _bingo.)
        {
            
            Debug.Log(c_Bingo.c_bingo);
        }*/
    }

    // �� Ȥ�� �������� ������ ������ ��
    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if (_myPlayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else
        {
            Player player = null;
            if (_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }
}
