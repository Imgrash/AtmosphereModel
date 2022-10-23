using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLayer
{
    public float VolumeAll = 0;
    public static float LayerVolume=0;
    public float Mass = 0;
    public AirLayer Below=null, Above=null;
    public void setVolume()
    {
        VolumeAll = LayerVolume;
        if(Below != null)
        {
            VolumeAll += Below.VolumeAll;
        }
        if(Above != null)
        {
            Above.setVolume();
        }
    }
}
