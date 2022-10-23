using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentSpace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PieceOfAir.BelowSpaceY = GetComponentsInChildren<EarthTemperature>()[0].GetComponent<Transform>().position.y;
    }
}
