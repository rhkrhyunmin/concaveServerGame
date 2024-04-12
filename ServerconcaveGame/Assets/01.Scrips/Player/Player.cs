using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerId { get; set; }
    public static int StonePosition { get; set; }

    void Start()
    {
        StonePosition = 0;
    }


    void Update()
    {

    }
}
