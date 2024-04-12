using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(
            () => { return null; });
        public static int chunkSize { get; set; } = 4096; // ������ ���� �����

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(chunkSize);
            if (CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(chunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }



    public class SendBuffer
    {
        // [ ][ ][ ][ ][ ][u][ ][ ][ ][ ]
        byte[] _buffer; // 
        int _usedSize = 0; // ����� ������ ũ�� Ŀ��
        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }
        public ArraySegment<byte> Open(int reserveSize) // ��ŭ ���ۿ� �����Ұ��� �ִ�ġ ����
        {
            if (reserveSize > FreeSize) // ������������ �� ��û�� ���
                return null;

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize)   // ���� ������ ��, ���� ����� �� ����
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;  // uĿ�� �̵�
            return segment;
        }
        // SendBuffer�� Clean()�� ����.
        // ��ȸ�����θ� ���, ������ ��� ��Ƽ������ ȯ�濡�� SendBuffer�� ������ ������ ��� ���� ����.
    }
}
