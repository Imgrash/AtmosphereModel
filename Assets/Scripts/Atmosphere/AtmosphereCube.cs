using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereCube : MonoBehaviour
{
    public static float actual = 30f;
    public LayerMask mask;
    public AtmosphereCube Up, Down, Left, Right, Forward, Behind;
    public bool belowAll = false, aboveAll = false, leftest = false, rightest = false, forwardest = false, behindest = false;
    public AtmosphereCube LowestCube, HighestCube, LeftestCube, RightestCube, ForwardestCube, BehindestCube;

    public static float volumeOfAirWithBox = 120 * 120 * 120 * actual * actual * actual;
    public static float ActualRadius;
    public float massOfAir = 0, volumeOfAirWithoutBox = 0, averageAbsoluteTemperatureWithoutBox = 50, averageAbsoluteTemperatureWithBox = 50
            , pressureSelf = 0;
    public int PiecesOfAirCount = 0;

    public float averagePressure = 0;

    [SerializeField]
    public AirLayer Layer;

    public void countPressureSelf()
    {
        float prUp = 0.15f;
        if (!aboveAll)
            prUp = Up.pressureSelf;
        pressureSelf = prUp +
            (massOfAir / 0.02898f) * 8.31f * averageAbsoluteTemperatureWithBox / volumeOfAirWithBox;
        if(!belowAll)
            Down.countPressureSelf();
    }

    public float HeatConductivityNow;
    static float[] HeatConductivity = { 0.0172f, 0.0181f, 0.0181f, 0.0189f, 0.0189f, 0.0198f, 0.0198f, 0.0206f, 0.0206f, 0.0214f, 0.0214f, 0.0223f, 0.0223f, 0.0231f, 0.0231f, 0.0239f, 0.0239f, 0.0247f, 0.0247f, 0.0255f, 0.0255f, 0.0263f, 0.0263f, 0.027f, 0.0276f, 0.0279f, 0.0283f, 0.0286f, 0.029f };
    public void SetHeatConductivity()
    {
        int clampTemp = (int)Mathf.Clamp(averageAbsoluteTemperatureWithBox, 190, 330)-190;
        HeatConductivityNow = HeatConductivity[clampTemp/ 5];
    }

    public void AddPieceOfAir(float volume, float temperature)
    {
        averageAbsoluteTemperatureWithoutBox = (averageAbsoluteTemperatureWithoutBox * volumeOfAirWithoutBox + temperature * volume) / (volumeOfAirWithoutBox + volume);
        volumeOfAirWithoutBox += volume;
       
        if (volumeOfAirWithoutBox < volumeOfAirWithBox)
            averageAbsoluteTemperatureWithBox = (averageAbsoluteTemperatureWithoutBox * volumeOfAirWithoutBox) / (volumeOfAirWithBox);
        else
            averageAbsoluteTemperatureWithBox = averageAbsoluteTemperatureWithoutBox;

        massOfAir += PieceOfAir.Mass;
        SetHeatConductivity();
    }
    public void SubPieceOfAir(float volume, float temperature)
    {
        if (Mathf.Abs(volumeOfAirWithoutBox - volume) < 0.001 || volume > volumeOfAirWithoutBox)
        {
            averageAbsoluteTemperatureWithoutBox = 0;
            volumeOfAirWithoutBox = 0;
        }
        else
        {
            averageAbsoluteTemperatureWithoutBox = averageAbsoluteTemperatureWithoutBox + (averageAbsoluteTemperatureWithoutBox - temperature) * volume / (volumeOfAirWithoutBox - volume);
            volumeOfAirWithoutBox -= volume;
        }

        if (volumeOfAirWithoutBox < volumeOfAirWithBox)
        {
            if ( Mathf.Abs(volumeOfAirWithoutBox - volume) < 0.001 || volume > volumeOfAirWithoutBox)
                averageAbsoluteTemperatureWithBox = 0;
            else
                averageAbsoluteTemperatureWithBox = (averageAbsoluteTemperatureWithoutBox * volumeOfAirWithoutBox) / (volumeOfAirWithBox);
        }
        else
            averageAbsoluteTemperatureWithBox = averageAbsoluteTemperatureWithoutBox;

        massOfAir -= PieceOfAir.Mass;
        SetHeatConductivity();
    }
    public void ChangeOnePieceOfAir(float volumeWas, float volume, float temperatureWas, float temperature)
    {
        SubPieceOfAir(volumeWas, temperatureWas);
        AddPieceOfAir(volume, temperature);
    }

    public void St()
    {
        int koof = 100;
        Vector3 scale = GetComponent<Transform>().localScale;
        volumeOfAirWithBox = scale.x * scale.y * scale.z * actual * actual * actual;
        mask = LayerMask.GetMask("Atm");
        RaycastHit hitUp;

        if (Physics.Raycast(transform.position, transform.transform.TransformDirection(Vector3.up), out hitUp, mask))
        {
            AtmosphereCube ac = hitUp.collider.GetComponent<AtmosphereCube>();
            if (ac != null)
                Up = ac;
            else
                aboveAll = true;
        }
        else
            aboveAll = true;

        RaycastHit hitDown;

        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hitDown, mask))
        {
            AtmosphereCube ac = hitDown.collider.GetComponent<AtmosphereCube>();
            if (ac != null)
                Down = ac;
            else
                belowAll = true;
        }
        else
              belowAll = true;

        RaycastHit hitRight;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitRight, mask))
        {
            AtmosphereCube ac = hitRight.collider.GetComponent<AtmosphereCube>();
            if (ac != null)
                Right = ac;
            else
                rightest = true;
        }
        else
            rightest = true;

        RaycastHit hitLeft;
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.right), out hitLeft, mask))
        {
            AtmosphereCube ac = hitLeft.collider.GetComponent<AtmosphereCube>();
            if (ac != null)
                Left = ac;
            else
                leftest = true;
        }
        else
        {
            leftest = true;
        }

        RaycastHit hitForward;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitForward, mask))
        {
            AtmosphereCube ac = hitForward.collider.GetComponent<AtmosphereCube>();
            if (ac != null)
                Forward = ac;
            else
                forwardest = true;
        }
        else
            forwardest = true;

        RaycastHit hitBehind;
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hitBehind, mask))
        {
            AtmosphereCube ac = hitBehind.collider.GetComponent<AtmosphereCube>();
            if (ac != null)
                Behind = ac;
            else
                behindest = true;
        }
        else
            behindest = true;
    }

    public void FindEsts()
    {
        LeftestCube = FindLeftest();
        RightestCube = FindRightest();
        LowestCube = FindLowest();
        HighestCube = FindHighest();
        BehindestCube = FindBehindest();
        ForwardestCube = FindForwardest();
    }
    public AtmosphereCube FindLeftest()
    {
        if (leftest)
            return this;
        return Left.FindLeftest();
    }
    public AtmosphereCube FindRightest()
    {
        if (rightest)
            return this;
        return Right.FindRightest();
    }
    public AtmosphereCube FindLowest()
    {
        if (belowAll)
            return this;
        return Down.FindLowest();
    }
    public AtmosphereCube FindHighest()
    {
        if (aboveAll)
            return this;
        return Up.FindHighest();
    }
    public AtmosphereCube FindBehindest()
    {
        if (behindest)
            return this;
        return Behind.FindBehindest();
    }
    public AtmosphereCube FindForwardest()
    {
        if (forwardest)
            return this;
        return Forward.FindForwardest();
    }
}
