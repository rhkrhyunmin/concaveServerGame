using UnityEngine;
using System.Collections;

// �̺�Ʈ ����.
public enum NetEventType
{
    Connect = 0,    
    Disconnect,   
    SendError,    
    ReceiveError,  
}

// �̺�Ʈ ���.
public enum NetEventResult
{
    Failure = -1,   
    Success = 0, 
}

// �̺�Ʈ ���� ����.
public class NetEventState
{
    public NetEventType type;  
    public NetEventResult result;   
}
