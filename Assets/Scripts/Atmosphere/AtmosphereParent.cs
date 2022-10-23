using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereParent : MonoBehaviour
{
    public static bool startTemp = false;
    public bool startTempSwitch = false;
    public float Mass;
    private void Update()
    {
        startTemp = startTempSwitch;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (AtmosphereCube child in GetComponentsInChildren<AtmosphereCube>())
        {
            child.St();
        }
        foreach (AtmosphereCube child in GetComponentsInChildren<AtmosphereCube>())
        {
            child.FindEsts();
        }
        foreach (AtmosphereCube child in GetComponentsInChildren<AtmosphereCube>())
        {
            if(child.aboveAll)
                child.countPressureSelf();
        }

        List<AtmosphereCube> BelowChildren = new List<AtmosphereCube>();
        
        foreach (AtmosphereCube child in GetComponentsInChildren<AtmosphereCube>())
        {
            if (!(child.behindest || child.forwardest || child.leftest || child.rightest) && child.belowAll)
                BelowChildren.Add(child);
        }
        foreach(AtmosphereCube child in BelowChildren)
        {
            AirLayer.LayerVolume += AtmosphereCube.volumeOfAirWithBox;
        }
        AirLayer.LayerVolume /= 1.1f;
        AtmosphereCube AC = BelowChildren[0];        
        AirLayer AL = new AirLayer(), tempAL = AL;
        AC = AC.Up;
        while(AC != null)
        {
            tempAL.Above = new AirLayer();
            tempAL.Above.Below = tempAL;
            tempAL = tempAL.Above;
            AC = AC.Up;
        }

        foreach (AtmosphereCube child in BelowChildren)
        {
            AtmosphereCube bc = child;
            tempAL = AL;
            while(bc != null)
            {
                bc.Layer = tempAL;
                bc = bc.Up;
                tempAL = tempAL.Above;
            }
        }

        BelowChildren.Clear();
        foreach (AtmosphereCube child in GetComponentsInChildren<AtmosphereCube>())
        {
            if (!(child.behindest || child.forwardest || child.leftest || child.rightest) && child.belowAll)
                BelowChildren.Add(child);
        }
        AtmosphereCube bc2 = BelowChildren[0];
        bc2.Layer.setVolume();
        while(bc2 != null)
        {
            bc2 = bc2.Up;
        }

        PieceOfAir.Mass = Mass;
        PieceOfAir.mg = Vector3.down * PieceOfAir.Mass * 9.8f;

        AtmosphereCube.ActualRadius = Mathf.Pow(AtmosphereCube.volumeOfAirWithBox, 1f / 3f);

        //Vector3 v3 = GetComponentsInChildren<AtmosphereCube>()[0].GetComponent<Transform>().localScale;
    }
}
