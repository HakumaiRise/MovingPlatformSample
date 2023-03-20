using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovingPlatform : MonoBehaviour
{
    //Rotate関連
    [Header("Rotate")]
    [SerializeField] RotType rottype;
    [SerializeField]
    enum RotType
    {
        Off,
        Loop,
        Normal,
        Smooth,
    }
    [SerializeField] Vector3 loopRotSpeed = Vector3.zero;
    [SerializeField] Vector3[] pointR = new Vector3[2];
    [SerializeField] float SpeedR;
    [SerializeField] int waitFrameR;
    int waitR;
    int pointNumR;
    bool isRotating = false;
    Vector3 defaultRot;
    float runTimeR = 0;

    //Move関連
    [Header("Move")]
    [SerializeField] MoveType movetype;
    [SerializeField]
    enum MoveType
    {
        Off,
        Normal,
        Smooth,
        SineCosine,
    }
    [SerializeField] Vector3[] pointM = new Vector3[2];
    int pointNumM;
    [SerializeField] float SpeedM;
    [SerializeField] int waitFrameM;
    int waitM;
    bool isMoving = false;
    Vector3 defaultPos;
    [Range(0, 360)]
    [SerializeField] float startAngM;
    [SerializeField] Vector3 sinAmplitudeM;
    [SerializeField] Vector3 cosAmplitudeM;
    [Range(0.001f, 20)]
    [SerializeField] float timeM = 1;
    float runTimeM = 0;

    //Scale関連
    [Header("Scale")]
    [SerializeField] ScaType scatype;
    [SerializeField]
    enum ScaType
    {
        Off,
        Normal,
        Smooth,
        SineCosine,
    }
    [SerializeField] Vector3[] pointS = new Vector3[2];
    int pointNumS;
    [SerializeField] float SpeedS;
    [SerializeField] int waitFrameS;
    int waitS;
    Vector3 defaultSca;
    [Range(0, 360)]
    [SerializeField] float startAngS;
    [SerializeField] Vector3 sinAmplitudeS;
    [SerializeField] Vector3 cosAmplitudeS;
    [Range(0.001f, 20)]
    [SerializeField] float timeS = 1;
    float runTimeS = 0;

    //Ride関連
    [Header("RideTrigger")]
    [SerializeField] bool useRideTrigger = false;
    bool canMove = false;
    [SerializeField] int gakuFrame;
    bool gaking = false;
    int gakF;

    //Player関連
    Platforming platforming;
    GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        platforming = player.GetComponent<Platforming>();
        if (useRideTrigger)
        {
            canMove = false;
        }
        if (timeM < 0.001)
        {
            timeM = 0.001f;
        }
        if (rottype == RotType.Normal || rottype == RotType.Smooth)
        {
            transform.localEulerAngles = Quaternion.Euler(pointR[0]).eulerAngles;
        }
        if (movetype == MoveType.Normal || movetype == MoveType.Smooth)
        {
            transform.localPosition = pointM[0];
        }
        if (scatype == ScaType.Normal || scatype == ScaType.Smooth)
        {
            transform.localScale = pointS[0];
        }
        defaultSca = transform.localScale;
        defaultPos = transform.localPosition;
        defaultRot = transform.localEulerAngles;
    }

    void FixedUpdate()
    {
        if (useRideTrigger)
        {
            if (canMove)
            {
                GakuGaku();
                if (gaking)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            canMove = false;
            gakF = 0;
        }

        Rotating();

        Moving();

        Scaling();

        ScaleZeroCheck();

    }

    private void OnCollisionStay(Collision collision)
    {
        if (platforming.plat != null)
        {
            if (platforming.plat.name == gameObject.name)
            {
                if (useRideTrigger && !canMove)
                {
                    canMove = true;
                }
            }
        }
    }

    public void Rotating()
    {
        if (rottype == RotType.Off)
        {
            return;
        }

        if (rottype == RotType.Loop)
        {
            RotLoop();
            return;
        }

        if (rottype == RotType.Normal)
        {
            RotNormal();
            return;
        }

        if (rottype == RotType.Smooth)
        {
            RotSmooth();
            return;
        }
    }

    public void Moving()
    {
        if (movetype == MoveType.Off)
        {
            return;
        }

        if (movetype == MoveType.Normal)
        {
            MoveNormal();
            return;
        }

        if (movetype == MoveType.Smooth)
        {
            MoveSmooth();
            return;
        }

        if (movetype == MoveType.SineCosine)
        {
            MoveSinCos();
            return;
        }
    }

    public void Scaling()
    {

        if (scatype == ScaType.Off)
        {
            return;
        }


        if (scatype == ScaType.SineCosine)
        {
            ScaSinCos();
        }

        if (scatype == ScaType.Normal)
        {
            ScaNormal();
        }

        if (scatype == ScaType.Smooth)

        {
            ScaSmooth();
        }
    }

    public void RotLoop()
    {
        this.transform.Rotate(50 * loopRotSpeed.x * Time.deltaTime, 50 * loopRotSpeed.y * Time.deltaTime, 50 * loopRotSpeed.z * Time.deltaTime);
    }

    public void RotNormal()
    {
        if (pointR.Length == 0)
        {
            return;
        }

        if (transform.localEulerAngles == Quaternion.Euler(pointR[pointNumR]).eulerAngles)
        {
            if (isRotating)
            {
                isRotating = false;
                runTimeR = 0;
                waitR = waitFrameR;
            }
            if (waitR > 0)
            {
                waitR--;
            }
            else
            {
                waitR = 0;
                isRotating = true;
                pointNumR++;
                if (pointNumR == pointR.Length)
                {
                    pointNumR = 0;
                }
            }
        }
        else
        {
            Vector3 start;
            if (pointNumR == 0)
            {
                start = pointR[pointR.Length - 1];
            }
            else
            {
                start = pointR[pointNumR - 1];
            }
            Vector3 end = pointR[pointNumR];
            float distance = Vector3.Distance(start, end);
            float location = (runTimeR * SpeedR * 50) / distance;
            transform.localEulerAngles = Vector3.Lerp(start, end, location);

            runTimeR = runTimeR + Time.deltaTime;
        }
    }

    public void RotSmooth()
    {
        if (pointR.Length == 0)
        {
            return;
        }

        if (transform.localEulerAngles == Quaternion.Euler(pointR[pointNumR]).eulerAngles)
        {
            if (isRotating)
            {
                isRotating = false;
                runTimeR = 0;
                waitR = waitFrameR;
            }
            if (waitR > 0)
            {
                waitR--;
            }
            else
            {
                waitR = 0;
                isRotating = true;
                pointNumR++;
                if (pointNumR == pointR.Length)
                {
                    pointNumR = 0;
                }
            }
        }
        else
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, pointR[pointNumR], SpeedR * runTimeR / 10);
            float dx = transform.localEulerAngles.x - Quaternion.Euler(pointR[pointNumR]).eulerAngles.x;
            float dy = transform.localEulerAngles.y - Quaternion.Euler(pointR[pointNumR]).eulerAngles.y;
            float dz = transform.localEulerAngles.z - Quaternion.Euler(pointR[pointNumR]).eulerAngles.z;
            if (-0.1 < dx && dx < 0.1)
            {
                transform.localEulerAngles = new Vector3(Quaternion.Euler(pointR[pointNumR]).eulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
            if (-0.1 < dy && dy < 0.1)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Quaternion.Euler(pointR[pointNumR]).eulerAngles.y, transform.localEulerAngles.z);
            }
            if (-0.1 < dz && dz < 0.1)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Quaternion.Euler(pointR[pointNumR]).eulerAngles.z);
            }
            runTimeR = runTimeR + Time.deltaTime;
        }
    }

    public void MoveNormal()
    {
        if (pointM.Length == 0)
        {
            return;
        }

        if (transform.localPosition == pointM[pointNumM])
        {
            if (isMoving)
            {
                isMoving = false;
                runTimeM = 0;
                waitM = waitFrameM;
            }
            if (waitM > 0)
            {
                waitM--;
            }
            else
            {
                waitM = 0;
                isMoving = true;
                pointNumM++;
                if (pointNumM == pointM.Length)
                {
                    pointNumM = 0;
                }
            }
        }
        else
        {
            Vector3 start;
            if (pointNumM == 0)
            {
                start = pointM[pointM.Length - 1];
            }
            else
            {
                start = pointM[pointNumM - 1];
            }
            Vector3 end = pointM[pointNumM];
            float distance = Vector3.Distance(start, end);
            float location = (runTimeM * SpeedM) / distance;
            transform.localPosition = Vector3.Lerp(start, end, location);

            runTimeM = runTimeM + Time.deltaTime;
        }
    }

    public void MoveSmooth()
    {
        if (pointM.Length == 0)
        {
            return;
        }


        if (transform.localPosition == pointM[pointNumM])
        {
            if (isMoving)
            {
                isMoving = false;
                runTimeM = 0;
                waitM = waitFrameM;
            }
            if (waitM > 0)
            {
                waitM--;
            }
            else
            {
                waitM = 0;
                isMoving = true;
                pointNumM++;
                if (pointNumM == pointM.Length)
                {
                    pointNumM = 0;
                }
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, pointM[pointNumM], SpeedM * runTimeM / 10);
            float dx = transform.localPosition.x - pointM[pointNumM].x;
            float dy = transform.localPosition.y - pointM[pointNumM].y;
            float dz = transform.localPosition.z - pointM[pointNumM].z;
            if (-0.01 < dx && dx < 0.01)
            {
                transform.localPosition = new Vector3(pointM[pointNumM].x, transform.localPosition.y, transform.localPosition.z);
            }
            if (-0.01 < dy && dy < 0.01)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, pointM[pointNumM].y, transform.localPosition.z);
            }
            if (-0.01 < dz && dz < 0.01)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, pointM[pointNumM].z);
            }
            runTimeM = runTimeM + Time.deltaTime;
        }
    }

    public void MoveSinCos()
    {
        float sin = Mathf.Sin(2 * Mathf.PI / timeM * runTimeM + (startAngM / 180) * Mathf.PI);
        float cos = Mathf.Cos(2 * Mathf.PI / timeM * runTimeM + (startAngM / 180) * Mathf.PI);

        this.transform.localPosition = defaultPos + sin * sinAmplitudeM + cos * cosAmplitudeM;

        runTimeM = runTimeM + Time.deltaTime;
    }

    public void MoveFall()
    {
        transform.localPosition = transform.localPosition + 0.1f * runTimeM * runTimeM * SpeedM * Vector3.down;
        runTimeM = runTimeM + Time.deltaTime;
    }

    public void ScaSinCos()
    {
        float sin = Mathf.Sin(2 * Mathf.PI * 1 / timeS * runTimeS + (startAngS / 180) * Mathf.PI);
        float cos = Mathf.Cos(2 * Mathf.PI * 1 / timeS * runTimeS + (startAngS / 180) * Mathf.PI);

        this.transform.localScale = defaultSca + sin * sinAmplitudeS + cos * cosAmplitudeS;

        runTimeS = runTimeS + Time.deltaTime;
    }

    public void ScaNormal()
    {
        if (pointS.Length == 0)
        {
            return;
        }

        if (transform.localScale == pointS[pointNumS])
        {
            if (isMoving)
            {
                isMoving = false;
                runTimeS = 0;
                waitS = waitFrameS;
            }
            if (waitS > 0)
            {
                waitS--;
            }
            else
            {
                waitS = 0;
                isMoving = true;
                pointNumS++;
                if (pointNumS == pointS.Length)
                {
                    pointNumS = 0;
                }
            }
        }
        else
        {
            Vector3 start;
            if (pointNumS == 0)
            {
                start = pointS[pointS.Length - 1];
            }
            else
            {
                start = pointS[pointNumS - 1];
            }
            Vector3 end = pointS[pointNumS];
            float distance = Vector3.Distance(start, end);
            float location = (runTimeS * SpeedS) / distance;
            transform.localScale = Vector3.Lerp(start, end, location);

            runTimeS = runTimeS + Time.deltaTime;
        }
    }

    public void ScaSmooth()
    {
        if (pointS.Length == 0)
        {
            return;
        }


        if (transform.localScale == pointS[pointNumS])
        {
            if (isMoving)
            {
                isMoving = false;
                runTimeS = 0;
                waitS = waitFrameS;
            }
            if (waitS > 0)
            {
                waitS--;
            }
            else
            {
                waitS = 0;
                isMoving = true;
                pointNumS++;
                if (pointNumS == pointS.Length)
                {
                    pointNumS = 0;
                }
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, pointS[pointNumS], SpeedS * runTimeS / 10);
            float dx = transform.localScale.x - pointS[pointNumS].x;
            float dy = transform.localScale.y - pointS[pointNumS].y;
            float dz = transform.localScale.z - pointS[pointNumS].z;
            if (-0.01 < dx && dx < 0.01)
            {
                transform.localScale = new Vector3(pointS[pointNumS].x, transform.localScale.y, transform.localScale.z);
            }
            if (-0.01 < dy && dy < 0.01)
            {
                transform.localScale = new Vector3(transform.localScale.x, pointS[pointNumS].y, transform.localScale.z);
            }
            if (-0.01 < dz && dz < 0.01)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, pointS[pointNumS].z);
            }
            runTimeS = runTimeS + Time.deltaTime;
        }
    }

    public void GakuGaku()
    {
        if (gakuFrame <= 0)
        {
            return;
        }
        if (!gaking && gakF == 0)
        {
            gaking = true;
            gakF = gakuFrame;
        }
        if (gakF > 0)
        {
            gakF--;
            float dy = ((gakF / 2 % 2) - 0.5f) * 0.1f; // -0.05 or 0.05
            transform.localPosition = transform.localPosition + new Vector3(0, dy, 0);

        }
        else
        {
            gakF = -1;
        }
        if (gakF == -1 && gaking)
        {
            gaking = false;
            transform.localPosition = defaultPos;
        }

    }

    public void ScaleZeroCheck()
    {
        if ((transform.localScale.x <= 0 || transform.localScale.y <= 0 || transform.localScale.z <= 0))
        {
            this.gameObject.SetActive(false);
        }
    }
}