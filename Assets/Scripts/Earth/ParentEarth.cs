using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentEarth : MonoBehaviour
{
    public void Start()
    {
        PieceOfAir.AboveEarthY = GetComponentsInChildren<EarthTemperature>()[0].GetComponent<Transform>().position.y;
    }
}
