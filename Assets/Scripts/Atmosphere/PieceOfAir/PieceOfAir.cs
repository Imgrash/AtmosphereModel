using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceOfAir : MonoBehaviour
{
    public static float actual = 30f;
    public AtmosphereCube wasCube = null;
    public List<GameObject> GOinTrigger = new List<GameObject>();
    public static float cuberoot = 1.0f / 3.0f;
    public static float Mass, AboveEarthY, BelowSpaceY;
    public static Vector3 mg;
    public Vector3 ArchimedForce = Vector3.zero, velocityPressureComponent = Vector3.zero;
    public float gigPlotn = 0;
    public float Plotn = 0;
    public float LayerBelowMass = 0;

    private Rigidbody rb;
    
    public float radius, ActualRadius;

    public float Volume = 10000f;
    public float pressureWas = 101330f, Pressure = 101330f;
    private float pressureLeftWas = 0, pressureLeft = 0
        , pressureRightWas = 0, pressureRight = 0
        , pressureForwardWas = 0, pressureForward = 0
        , pressureBehindWas = 0, pressureBehind = 0
        , pressureBelowWas = 0, pressureBelow = 0
        , tempCels = 15f;
    public float TempKelv = 273.15f;
    public float pVT;

    float HeatConductivityNow;
    static float[] HeatConductivity = { 0.0172f, 0.0181f, 0.0181f, 0.0189f, 0.0189f, 0.0198f, 0.0198f, 0.0206f, 0.0206f, 0.0214f, 0.0214f, 0.0223f, 0.0223f, 0.0231f, 0.0231f, 0.0239f, 0.0239f, 0.0247f, 0.0247f, 0.0255f, 0.0255f, 0.0263f, 0.0263f, 0.027f, 0.0276f, 0.0279f, 0.0283f, 0.0286f, 0.029f };
    //static Material[] HeatMaterial = { new Material(,)}

    public AtmosphereCube atmosphereCube;

    public void SetHeatConductivity()
    {
        int clampTemp = (int)Mathf.Clamp(TempKelv,200,340)-200;
        HeatConductivityNow = HeatConductivity[clampTemp/5];

        float tra = 0.7f;
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color((clampTemp) / 140f, 0, 1f - clampTemp / 140f, tra*((clampTemp) / 140f)));
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.position += new Vector3((Random.value - 0.5f)*1600f, (Random.value - 0.5f) * 1000f, (Random.value - 0.5f)*1600f);        

        pVT = Pressure * Volume / TempKelv;
        tempCels = TempKelv - 273.15f;
        ActualRadius = Mathf.Pow(Volume, cuberoot);
        radius = ActualRadius / actual;
        transform.localScale = new Vector3(radius, radius, radius);
        SetHeatConductivity();

        Plotn = Mass / Volume;
    }
    
    public void AddEnergy(float energy)
    {
        tempCels += energy / (Mass * 1000f);
        float tempWas = TempKelv;
        TempKelv = 273.15f + tempCels;
        float volumeWas = Volume;
        Volume = pVT * TempKelv / Pressure;
        ActualRadius = Mathf.Pow(Volume, cuberoot);
        radius = ActualRadius / actual;
        transform.localScale = new Vector3(radius, radius, radius);

        SetHeatConductivity();
        if(atmosphereCube != null)
        {
            atmosphereCube.ChangeOnePieceOfAir(volumeWas, Volume, tempWas, TempKelv);
            atmosphereCube.countPressureSelf();
        }
        Plotn = Mass / Volume;
    }

    public float CountEnergy(float Radius2, float T2)
    {
        float A = Radius2;
        if(ActualRadius < A)
            A = ActualRadius;
        return ((HeatConductivityNow) * (A * A) * (T2 - TempKelv)/A)*20000;
    } 

    public void addForceFromPressure() 
    {
        velocityPressureComponent = Vector3.zero;
        if (atmosphereCube != null)
        {
            if(!atmosphereCube.behindest)
                velocityPressureComponent += Vector3.forward * atmosphereCube.Behind.pressureSelf * ActualRadius*ActualRadius;
            else
                velocityPressureComponent += Vector3.forward * atmosphereCube.ForwardestCube.Behind.pressureSelf * ActualRadius * ActualRadius;

            if (!atmosphereCube.forwardest)
                velocityPressureComponent += -Vector3.forward * atmosphereCube.Forward.pressureSelf * ActualRadius * ActualRadius;
            else
                velocityPressureComponent += -Vector3.forward * atmosphereCube.BehindestCube.Forward.pressureSelf * ActualRadius * ActualRadius;

            if (!atmosphereCube.leftest)
                velocityPressureComponent += -Vector3.left * atmosphereCube.Left.pressureSelf * ActualRadius * ActualRadius;
            else
                velocityPressureComponent += -Vector3.left * atmosphereCube.RightestCube.Left.pressureSelf * ActualRadius * ActualRadius;

            if (!atmosphereCube.rightest)
                velocityPressureComponent += -Vector3.right * atmosphereCube.Right.pressureSelf * ActualRadius * ActualRadius;
            else
                velocityPressureComponent += -Vector3.right * atmosphereCube.LeftestCube.Right.pressureSelf * ActualRadius * ActualRadius;
        }
    }
    public void PressureEffectVolume()
    {
        float volumeWas = Volume;
        Volume = pVT * TempKelv / Pressure;
        ActualRadius = Mathf.Pow(Volume, cuberoot);
        radius = ActualRadius / actual;
        transform.localScale = new Vector3(radius, radius, radius);
        if(atmosphereCube != null && atmosphereCube.Layer != null)
        {
            atmosphereCube.ChangeOnePieceOfAir(volumeWas, Volume, TempKelv, TempKelv);
            atmosphereCube.countPressureSelf();

            ArchimedForce = Vector3.up * (atmosphereCube.Layer.Mass / AirLayer.LayerVolume) * 9.8f * Volume;
            gigPlotn = atmosphereCube.Layer.Mass / AirLayer.LayerVolume;
            LayerBelowMass = atmosphereCube.Layer.Mass;
        }

        
        Plotn = Mass / Volume;
    }

    public void getPressure()
    {
        if (atmosphereCube != null)
        {
            Pressure = atmosphereCube.pressureSelf;
            if (!atmosphereCube.behindest)
                pressureBehind = atmosphereCube.Behind.pressureSelf;
            else
                pressureBehind = atmosphereCube.ForwardestCube.Behind.pressureSelf;

            if (!atmosphereCube.forwardest)
                pressureForward = atmosphereCube.Forward.pressureSelf;
            else
                pressureForward = atmosphereCube.BehindestCube.Forward.pressureSelf;

            if (!atmosphereCube.leftest)
                pressureLeft = atmosphereCube.Left.pressureSelf;
            else
                pressureLeft = atmosphereCube.RightestCube.Left.pressureSelf;

            if (!atmosphereCube.rightest)
                pressureRight = atmosphereCube.Right.pressureSelf;
            else
                pressureRight = atmosphereCube.LeftestCube.Right.pressureSelf;

            if (!atmosphereCube.belowAll)
                pressureBelow = atmosphereCube.Down.pressureSelf - atmosphereCube.pressureSelf;
            else
                pressureBelow = 0;
        }
        else
            pressureBelow = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (atmosphereCube != null)
        {
            if (transform.position.x > atmosphereCube.RightestCube.transform.position.x)
            {
                transform.Translate(atmosphereCube.LeftestCube.Right.transform.position - atmosphereCube.transform.position);
            }
            else if (transform.position.x < atmosphereCube.LeftestCube.transform.position.x)
            {
                transform.Translate(atmosphereCube.RightestCube.Left.transform.position - atmosphereCube.transform.position);
            }
            else if (transform.position.z > atmosphereCube.ForwardestCube.transform.position.z)
            {
                transform.Translate(atmosphereCube.BehindestCube.Forward.transform.position - atmosphereCube.transform.position);
            }
            else if (transform.position.z < atmosphereCube.BehindestCube.transform.position.z)
            {
                transform.Translate(atmosphereCube.ForwardestCube.Behind.transform.position - atmosphereCube.transform.position);
            }

            if (AboveEarthY > transform.position.y)
                transform.position = new Vector3(transform.position.x, AboveEarthY, transform.position.z);
            else if (BelowSpaceY < transform.position.y)
                transform.position = new Vector3(transform.position.x, BelowSpaceY, transform.position.z);
        }
        
        if (atmosphereCube != null)
        {
            getPressure();
            if (pressureWas != Pressure || pressureBelowWas != pressureBelow)
            {
                PressureEffectVolume();
                pressureWas = Pressure;
                pressureBelowWas = pressureBelow;
            }

            
            if (pressureLeft != pressureLeftWas
                || pressureRight != pressureRightWas
                || pressureForward != pressureForwardWas
                || pressureBehind != pressureBehindWas)
            {
                addForceFromPressure();
                pressureLeftWas = pressureLeft;
                pressureRightWas = pressureRight;
                pressureForwardWas = pressureForward;
                pressureBehindWas = pressureBehind;
            }
            
        }
        else
        {
            getPressure();
            if (pressureWas != Pressure || pressureBelowWas != pressureBelow)
            {
                PressureEffectVolume();
                pressureWas = Pressure;
                pressureBelowWas = pressureBelow;
            }
        }

        /*
        foreach(GameObject other in GOinTrigger)
        {
            if (gameObject != null)
            {
                Rigidbody rrr = other.GetComponent<Rigidbody>();
                Vector3 otPos = other.transform.position;
                float otherRadius = other.GetComponent<PieceOfAir>().ActualRadius;

                PieceOfAir otAir = other.GetComponent<PieceOfAir>();
                if (wasCube != null)
                {
                    rb.velocity -= Vector3.Normalize(otPos - transform.position) * ((Mass / 0.02898f) * 8.31f * otAir.TempKelv / otAir.Volume) / Mass * ActualRadius * Time.deltaTime;
                    rrr.velocity -= Vector3.Normalize(transform.position - otPos) * ((Mass / 0.02898f) * 8.31f * TempKelv / Volume) / Mass * otherRadius * Time.deltaTime;
                }

                other.GetComponent<PieceOfAir>().AddEnergy(Time.deltaTime * CountEnergy(HeatConductivityNow, ActualRadius, TempKelv)/5f);
            }
            
        }*/
        if (AtmosphereParent.startTemp && atmosphereCube != null)
            AddEnergy(Time.deltaTime * CountEnergy(AtmosphereCube.ActualRadius, atmosphereCube.averageAbsoluteTemperatureWithBox) / 50f);
        rb.velocity += (velocityPressureComponent* ActualRadius* ActualRadius/500000000f + ArchimedForce + mg) / Mass * Time.deltaTime;
    }

    private void OnTriggerExit(Collider other)
    {        
        if (other.CompareTag("Earth"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);// = Vector3.zero;//-= (velocityPressureComponent + ArchimedForce + mg) / Mass;
            transform.position = new Vector3(transform.position.x, AboveEarthY  + transform.lossyScale.y * 0.01f, transform.position.z);
            if (AtmosphereParent.startTemp)
                AddEnergy(Time.deltaTime * CountEnergy(ActualRadius, other.GetComponent<EarthTemperature>().TempKelvin) * 20);
        }
        if (other.CompareTag("Space"))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);//= Vector3.zero;//-= (velocityPressureComponent + ArchimedForce + mg) / Mass;
            transform.position = new Vector3(transform.position.x, BelowSpaceY - transform.lossyScale.y - transform.lossyScale.y * 0.01f, transform.position.z);
            if (AtmosphereParent.startTemp)
                AddEnergy(Time.deltaTime * CountEnergy(ActualRadius, 210f) * 20);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (AtmosphereParent.startTemp)
        {
            if (other.CompareTag("Earth"))
                AddEnergy(Time.deltaTime * CountEnergy(ActualRadius, other.GetComponent<EarthTemperature>().TempKelvin) * 20);
            if (other.CompareTag("Space"))
                AddEnergy(Time.deltaTime * CountEnergy(ActualRadius, 210f) * 20);
        }
        if (other.CompareTag("AtmosphereCube"))
        {
            AtmosphereCube oth = other.GetComponent<AtmosphereCube>();
            if((atmosphereCube == null || oth != atmosphereCube)&&oth.Layer != null)
            {
                oth.Layer.Mass += Mass;
                if (atmosphereCube != null)
                    atmosphereCube.Layer.Mass -= Mass;
                oth.AddPieceOfAir(Volume, TempKelv);
                oth.countPressureSelf();
                if (atmosphereCube != null)
                {
                    atmosphereCube.SubPieceOfAir(Volume, TempKelv);
                    atmosphereCube.countPressureSelf();
                }
                PressureEffectVolume();
                atmosphereCube = oth;
            }
        }
    }
}
