using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        // ���Ŭ�������� ���� ���ϵ��� ��(����)
        // ��Ŷ���� : [size(2)][packetId(2)][����......] 
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            while (true)
            {
                // �ּ��� ���(������)�� ���� �� �ִ��� Ȯ��
                if (buffer.Count < HeaderSize)
                    break;
                // ���(size(2))�� �а�, ������ �����Ʈ ¥�� ��Ŷ���� Ȯ�� 
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)    // �κ��� ����
                    break;
                // ��Ŷ ����, OnRecvPacket�� ���� ��Ŷ��ŭ ���� (�Ű����� �ؼ� : ����, ������, ����)
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                // ���� �Ϸ�Ǹ� ��Ŷ �б� Ŀ�� �̵�
                processLen += dataSize;
                // ������� ���� ��Ŷ 1��, �б� ������ ��

                // ������� ó���� ��Ŷ�� �����͸� ���ۿ��� �����ϰ� ���� �����͸� ���ο� ���ۿ� �Ҵ�
                buffer = new ArraySegment<byte>(
                    buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
                UnityEngine.Debug.Log(buffer); 
            }
            // ������� ó���� ���� ������ ũ�� ��ȯ
            return processLen;
        }
        // ���Ŭ�������� OnRecv�� �̰����� ��ü�Ͽ� ����ϵ��� ����
        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        List<ArraySegment<byte>> _pendinglist = new List<ArraySegment<byte>>();
        object _lock = new object();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnSend(int numOfBytes);
        public abstract int OnRecv(ArraySegment<byte> buffer);


        public void Start(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted); // (2-2) ���ô� ���
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            // RegisterSend();

            RegisterRecv(); // (1) ���Ŵ��
        }
        #region ������ ����
        // 1. ������
        void RegisterRecv()
        {
            _recvBuffer.Clean(); // Ŀ�� �ڷ� �̵� ����
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);    // (2-1) ���ô� ���ø���(�����ͼ��� �߻�)
        }
        // 2. �����ͼ���
        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {   // ����1 : ���� �����Ʈ�� �޾Ҵ°�? (������ ������ 0����Ʈ ����)
            // ����2 : ���ῡ Ư���� ���� ������ üũ
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                // TODO
                try
                {
                    // Write Ŀ�� �̵�
                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // ������ ������ �����͸� �Ѱ��ְ� �󸶳� ó���ߴ��� �޴´�
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }
                    // Read Ŀ�� �̵�
                    if (_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv(); // (3) ���ô� �ٽ� ������(�̺�Ʈ ��ȣ��)
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed! {e}");
                }
            }
            else
            {
                // Disconnect
            }
        }
        #endregion

        #region �����ͼ۽�
        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendinglist.Count == 0)    // ������� ���� ������
                    RegisterSend();
                
            }

        }

        void RegisterSend()
        {
            while (_sendQueue.Count > 0) // SendQueue�� �������� �ݺ�
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendinglist.Add(buff);
            }
            _sendArgs.BufferList = _pendinglist;

            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)    // �ٷ� ���� �� ���� ��
            {
                OnSendCompleted(null, _sendArgs);   // �̺�Ʈ ���� �ʱ�ȭ
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;    // �̺�Ʈ�� ���۸���Ʈ ����� ����
                        _pendinglist.Clear();           // ����

                        OnSend(_sendArgs.BytesTransferred);
                        //TODO
                        if (_sendQueue.Count > 0)   // Ȥ�� ���� SendQueue�� ���� �ִٸ� ����
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"OnReceiveCompleted Failed! {ex}");
                    }

                }
                else
                {
                    Disconnect();
                }
            }

        }



        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            {
                return;
            }
            OnDisconnected(_socket.RemoteEndPoint);
            // ��������
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #endregion
    }
}
