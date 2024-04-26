using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class PacketManager
    {
        #region �̱���
        // ��Ŷ�Ŵ����� ������ �Ͼ����Ƿ� �̱������� ������ ����
        static PacketManager _instance = new PacketManager();
        public static PacketManager Instance { get { return _instance; } }
        #endregion

        PacketManager()
        {
            Register();
        }

        Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc
            = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
        Dictionary<ushort, Action<PacketSession, IPacket>> _handler
            = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        public void Register()
        {   // ��Ƽ������ ���� ���� �ʿ�
            _makeFunc.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
            _handler.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);
            
            _makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
            _handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);
            
            _makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
            _handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);

           /* _makeFunc.Add((ushort)PacketID.S_BroadCastStone, MakePacket<S_Bingo>);
            _handler.Add((ushort)PacketID.S_BroadCastStone, PacketHandler.S_BroadCastStoneHandler);*/

        }

        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer
            , Action<PacketSession, IPacket> onRecvCallback = null)  //  Action �ݹ� : �ԷµǴ� �׼ǿ� ���� Invoke
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;    // id���� ������ switch ��� ��ųʸ����� ���� ã�� ��ϵ� �ڵ鷯���� �ش� �۾� Invoke

            Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
            if (_makeFunc.TryGetValue(id, out func))
            {
                IPacket packet = func.Invoke(session, buffer);
                if (onRecvCallback != null) // �׼��� ����Ǹ� Invoke 
                    onRecvCallback.Invoke(session, packet);
                else
                    HandlePacket(session, packet);  // ����Ʈ ����
            }

        }
        // ��Ŷ ���� �κ�
        T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
        {
            T packet = new T();     // ��Ŷ �����
            packet.Read(buffer);    // ���� ��Ŷ �б�
           
            return packet;
        }
        // ��Ŷ ó���κ� �и�
        public void HandlePacket(PacketSession session, IPacket packet)
        {
            Action<PacketSession, IPacket> action = null;
            if (_handler.TryGetValue(packet.Protocol, out action))
            {
                action.Invoke(session, packet);
            }
        }
    }
}
