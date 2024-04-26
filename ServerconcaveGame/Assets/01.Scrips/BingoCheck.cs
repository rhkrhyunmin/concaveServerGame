using DummyClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoCheck : MonoBehaviour
{
    private const int Block = 3;
    private int _idx = 0;
    private bool[] _bingoindex = new bool[Block * Block];

    [SerializeField] private Image[] _image;
    [SerializeField] private Sprite _circle;

    private void Start()
    {
        for (int i = 0; i < _bingoindex.Length; i++)
            _bingoindex[i] = false;
    }

    public void CheckLine(C_RandomIndex randIdx, bool value)
    {
        // randIdx ��ü�� values ����Ʈ�� �ִ� �� ������ �ε����� ����Ͽ� _bingoindex �迭�� ���� �Ҵ��մϴ�.
        foreach (int index in randIdx.values)
        {
            _bingoindex[index] = value; // �־��� value ���� �Ҵ��մϴ�.
            ShowCircle(index);
        }

        
        CheckBingo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _bingoindex[_idx] = true;
            ShowCircle(_idx);
            print(CheckBingo());
            _idx++;
        }
    }

    private void ShowCircle(int idx)
    {
        _image[idx].enabled = true;
        _image[idx].sprite = _circle;
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
            // ���� ���� üũ
            if (_bingoindex[i * 3] && _bingoindex[i * 3 + 1] && _bingoindex[i * 3 + 2])
                return true;

            // ���� ���� üũ
            if (_bingoindex[i] && _bingoindex[i + 3] && _bingoindex[i + 6])
                return true;
        }

        // �밢�� ���� üũ(������ ������ ���� �Ʒ���)
        if ((_bingoindex[0] && _bingoindex[4] && _bingoindex[8]))
            return true;

        // �밢�� ���� üũ(���� ������ ������ �Ʒ���)
        if ((_bingoindex[2] && _bingoindex[4] && _bingoindex[6]))
            return true;

        return false;
    }
}
