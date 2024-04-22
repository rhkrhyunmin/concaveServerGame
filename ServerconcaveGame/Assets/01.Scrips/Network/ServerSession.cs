using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient.Session
{

    class ServerSession : PacketSession // ���������� ����
    {                           // �������� �ۼ��� ��������, �ۼ��Ž��� ���� �ۼ�
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected bytes: {endPoint}");

        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected bytes: {endPoint}");
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)  // ���ú� �̺�Ʈ �߻��� ����
        {

            UnityEngine.Debug.Log($"Transferred bytes: qwerqwerqwer");
            PacketManager.Instance.OnRecvPacket(this, buffer, (s, p) => PacketQueue.Instance.Push(p));  // ��Ŷ�Ŵ��� ����(�ؼ��� ����)
            // (s, p) PacketSession, IPacket
        }
        public override void OnSend(int numOfBytes)             // ���� �̺�Ʈ �߻��� ����
        {
            UnityEngine.Debug.Log($"Transferred bytes: {numOfBytes}");
        }
    }
}
