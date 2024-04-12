using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "SO/BingoExample")]
public class BingoExSo : ScriptableObject
{
    [Header("우리반 번호")]
    public List<string> names;
}
