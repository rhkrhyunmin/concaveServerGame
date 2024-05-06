using DummyClient;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BingoCheck : MonoBehaviour
{
    public static BingoCheck Instance = new BingoCheck();

    [SerializeField]
    public const int Block = 3;
    [SerializeField]
    public int _idx = 0;
    public bool[] _bingoindex = new bool[Block * Block];

    private bool isBingo = false;

    public List<TextMeshProUGUI> Textvalue = new List<TextMeshProUGUI>();

    public Sprite _circle;

    public TextMeshProUGUI endText;

    private void Start()
    {
        for (int i = 0; i < _bingoindex.Length; i++)
            _bingoindex[i] = false;
    }

    public void Bingo(IPacket packet)
    {
        C_Bingo pkt = packet as C_Bingo;
        for(int i = 0; i < Textvalue.Count; i++)
        {
            if (pkt.c_bingo.ToString() == Textvalue[i].text)
            {
                Debug.Log(Textvalue[i].text);

                // 해당 텍스트의 하위에 있는 이미지들을 찾아서 활성화시킴
                
                Image[] imagesInChildren = Textvalue[i].GetComponentsInChildren<Image>();
                foreach (Image image in imagesInChildren)
                {
                    image.enabled = true;
                    image.sprite = _circle;
                    _bingoindex[i] = true;

                    CheckBingo();
                }
            }
            
        }
    }



    private void CheckIdx()
    {
        int[] idx = { 0, 0, 0, 0 };
        int a = 0, b = 0, c = 0;

        for (int i = 0; i < _bingoindex.Length; i++)
        {
            if (!_bingoindex[i]) continue;

            if (i % 4 == 0) idx[2]++;

            if (0 < i && i <= 6 && i % 2 == 0)
                idx[1]++;

            //if (i / block == 0) { a++; if (a >= block) idx[1] = a; }
            //else if (i / block == 1) { b++; if (b >= block) idx[1] = b; }
            //else { c++; if (c >= block) idx[1] = c; }

            if (i % Block == 0) idx[0]++;

            for (int crossUp = 0; crossUp < Block; crossUp++)
                if (i / 3 == crossUp) idx[3]++;
        }

        foreach (int i in idx)
        {
            if (i >= Block)
            {
                Debug.Log(i);
                //���� ��
                //�׽�Ʈ ���ߴµ� �Ǵ°ɷ� �ض� ��
            }
        }
    }

    private bool CheckBingo()
    {
        for (int i = 0; i < 3; i++)
        {
            // 가로 라인 체크
            if (_bingoindex[i * 3] && _bingoindex[i * 3 + 1] && _bingoindex[i * 3 + 2])
            {
                endText.text = "승리";
                return true;
            }

            // 세로 라인 체크
            if (_bingoindex[i] && _bingoindex[i + 3] && _bingoindex[i + 6])
            {
                endText.text = "승리";
                return true;
            }
        }

        // 대각선 라인 체크 (왼쪽 위에서 오른쪽 아래로)
        if ((_bingoindex[0] && _bingoindex[4] && _bingoindex[8]))
        {
            endText.text = "승리";
            return true;
        }

        // 대각선 라인 체크 (오른쪽 위에서 왼쪽 아래로)
        if ((_bingoindex[2] && _bingoindex[4] && _bingoindex[6]))
        {
            endText.text = "승리";
            return true;
        }

        return false;
    }
}
