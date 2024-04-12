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



        public static void S_BroadCastStoneHandler(PacketSession session, IPacket packet)
        {
            S_BroadCastStone pkt = packet as S_BroadCastStone;
            ServerSession serverSession = session as ServerSession;

            PlayerManager.Instance.BroadCastStone(pkt);
        }

    }
}


