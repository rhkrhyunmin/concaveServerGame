using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    public enum PacketID
    {
        S_BroadcastEnterGame = 1,
        C_LeaveGame = 2,
        S_BroadcastLeaveGame = 3,
        S_PlayerList = 4,

        C_RandomIndex = 5,
        S_BroadCastStone = 6,
        c_Bingo = 7,
    }

    public interface IPacket
    {
        ushort Protocol { get; }
        void Read(ArraySegment<byte> segment);
        ArraySegment<byte> Write();
    }

    public class C_RandomIndex : IPacket
    {
        public List<int> values { get; } = new List<int>();

        public ushort Protocol { get { return (ushort)PacketID.C_RandomIndex; } }

        public void Read(ArraySegment<byte> segment)
        {
            int count = 0;

            // 패킷 ID를 건너뜁니다.
            count += sizeof(ushort);

            // 값들의 개수를 읽어들입니다.
            /* int valueCount = BitConverter.ToInt32(segment.Array, segment.Offset + count);*/
            count += sizeof(ushort);
            // Console.WriteLine(valueCount);


            // 각 값을 읽어들입니다.
            for (int i = 0; i < 9; i++)
            {
                /*// 값의 길이를 읽어들입니다.
                int valueLength = BitConverter.ToInt32(segment.Array, segment.Offset + count);
                ;*/

                // 값 자체를 읽어들여 문자열로 변환합니다.
                int value = BitConverter.ToInt32(segment.Array, segment.Offset + count);
                this.values.Add(value);
                Console.WriteLine(value);
                count += sizeof(int);

                // 다음 값의 위치로 이동합니다.
                //count += valueLength;

            }
        }


        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            int count = 0; // ushort -> int로 변경

            count += sizeof(ushort);

            // 패킷 ID를 전송합니다.
            Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RandomIndex), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            // 각 값을 전송합니다.
            foreach (int value in this.values)
            {
                // 값 자체를 전송합니다.
                Array.Copy(BitConverter.GetBytes(value), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
            }

            // 전체 패킷의 길이를 전송합니다.
            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }


    }

    public class C_Bingo : IPacket
    {
        public int StonePosition;
        public ushort Protocol { get { return (ushort)PacketID.S_BroadCastStone; } }

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.StonePosition = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastStone), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.StonePosition), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }
    }


    public class S_Bingo : IPacket
    {
        public int playerId;

        public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEnterGame; } }

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastEnterGame : IPacket
    {
        public int playerId;

        public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEnterGame; } }

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }
    }

    public class C_LeaveGame : IPacket
    {
        public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

        public void Read(ArraySegment<byte> segment)
        {
            // LeaveGame 패킷은 별도의 데이터를 읽을 필요가 없습니다.
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }
    }

    public class S_BroadcastLeaveGame : IPacket
    {
        public int playerId;

        public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame; } }

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastLeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }
    }

    public class S_PlayerList : IPacket
    {
        public class Player
        {
            public bool isSelf;
            public int playerId;

            public void Read(ArraySegment<byte> segment, ref ushort count)
            {
                this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
                count += sizeof(bool);
                this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
                count += sizeof(int);
            }

            public bool Write(ArraySegment<byte> segment, ref ushort count)
            {
                Array.Copy(BitConverter.GetBytes(this.isSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
                count += sizeof(bool);
                Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                return true;
            }
        }
        public List<Player> players = new List<Player>();

        public ushort Protocol { get { return (ushort)PacketID.S_PlayerList; } }

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.players.Clear();
            ushort playerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            for (int i = 0; i < playerLen; i++)
            {
                Player player = new Player();
                player.Read(segment, ref count);
                players.Add(player);
            }
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PlayerList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            foreach (Player player in this.players)
                player.Write(segment, ref count);

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }
    }
}
