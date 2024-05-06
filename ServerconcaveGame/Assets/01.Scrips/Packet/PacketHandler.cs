using DummyClient.Session;
using ServerCore;
using UnityEngine;

namespace DummyClient
{
    class PacketHandler
    {
        // � ���ǿ��� ������ ��Ŷ����, � ������ ��Ŷ����

        // �÷��̾� ���� ��ȣ ������
        public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
        {
            S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
            ServerSession serverSession = session as ServerSession;

            PlayerManager.Instance.EnterGame(pkt);
        }

        // �÷��̾� ���� ��ȣ ������
        public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
        {
            S_BroadcastLeaveGame pkt = packet as S_BroadcastLeaveGame;
            ServerSession serverSession = session as ServerSession;

            PlayerManager.Instance.LeaveGame(pkt);
        }

        // ���� �÷��̾� ����Ʈ
        public static void S_PlayerListHandler(PacketSession session, IPacket packet)
        {
            S_PlayerList pkt = packet as S_PlayerList;
            ServerSession serverSession = session as ServerSession;

            PlayerManager.Instance.Add(pkt);
        }

        public static void C_BingoHandle(PacketSession session, IPacket packet)
        {
            C_Bingo pkt = packet as C_Bingo;
            ServerSession serverSession = session as ServerSession;
            
            string c_bingoAsString = pkt.c_bingo.ToString();
            if(GameManager.Instance.randomBingo.selectedItems.Contains(c_bingoAsString))
            {
                GameManager.Instance.checkBingo.Bingo(packet);
            }  
        }

        public static void C_EndTextHandle(PacketSession session, IPacket packet)
        {
            C_EndText pkt = packet as C_EndText;
            ServerSession serverSession = session as ServerSession;

            string victoryText = pkt.endText;
        }

        public static void S_BroadCastStoneHandler(PacketSession session, IPacket packet)
        {
            S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
            ServerSession serverSession = session as ServerSession;

            PlayerManager.Instance.BroadCastStone(pkt);
        }

    }
}


