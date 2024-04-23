using DummyClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoCheck : MonoBehaviour
{
    private const int block = 3;

    private bool[] _bingoindex = new bool[block * block];

    private void Start()
    {
        for (int i = 0; i < _bingoindex.Length; i++)
            _bingoindex[i] = false;
    }

    public void CheckLine(C_RandomIndex randIdx, bool value)
    {
        _bingoindex[randIdx.index] = value;

        CheckIdx();
    }

    private void CheckIdx()
    {
        int[] idx = { 0, 0, 0, 0 };
        int a = 0, b = 0, c = 0;

        for (int i = 0; i < _bingoindex.Length; i++)
        {
            if (!_bingoindex[i]) continue;

            if (i % 4 == 0) idx[2]++;

            if (i / block == 0) { a++; if (a >= 3) idx[1] = a; }
            else if (i / block == 1) { b++; if (b >= 3) idx[1] = b; }
            else { c++; if (c >= 3) idx[1] = c; }

            if (i % block == 0) idx[0]++;

            for (int crossUp = 1; crossUp <= 3; crossUp++)
                if (i / 2 == crossUp) idx[3]++;
        }

        foreach(int i in idx)
        {
            if (i > 3)
            {
                //됬을 때
                //테스트 안했는데 되는걸로 해라 걍
            }
        }
    }
}
