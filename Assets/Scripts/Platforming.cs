using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforming : MonoBehaviour
{
    public GameObject plat = null;
    public GameObject center = null;
    public bool m_isGround = false;

    [SerializeField] Vector3 origin;
    [SerializeField] float radius;
    [SerializeField] float footPos;
    [SerializeField] float groundCheckDistance_onPlat;
    [SerializeField] float groundCheckDistance_up;
    [SerializeField] float groundCheckDistance_down;

    Quaternion pastRot;
    Vector3 pastPos;
    Vector3 pastSca;
    Quaternion nowRot;
    Vector3 nowPos;
    Vector3 nowSca;

    public LayerMask m_layerMask;
    Rigidbody m_rigidbody;

    void Start()
    {
        m_rigidbody = transform.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (plat != null)
        {
            if (pastSca.x <= 0 || pastSca.y <= 0 || pastSca.z <= 0 || nowSca.x <= 0 || nowSca.y <= 0 || nowSca.z <= 0)
            {   //platのScaleが0以下の場合はMoveWithしない
                plat = null;
                center = null;
                return;
            }

            if (center != null)
            {   //Centerあり
                nowRot = center.transform.rotation;
                nowPos = center.transform.position;
                nowSca = center.transform.lossyScale;

                MoveWith();

                pastRot = center.transform.rotation;
                pastPos = center.transform.position;
                pastSca = center.transform.lossyScale;

            }
            else
            {   //Centerなし
                nowRot = plat.transform.rotation;
                nowPos = plat.transform.position;
                nowSca = plat.transform.lossyScale;

                MoveWith();

                pastRot = plat.transform.rotation;
                pastPos = plat.transform.position;
                pastSca = plat.transform.lossyScale;
            }
        }

        GroundCheck();

        //プレイヤーを垂直に戻す
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
    }
    public void MoveWith()
    {
        Vector3 player_pastPos = transform.position;

        //Rotate
        Quaternion deltaRot = nowRot * Quaternion.Inverse(pastRot);
        transform.RotateAround(nowPos, new Vector3(1, 0, 0), deltaRot.eulerAngles.x);
        transform.RotateAround(nowPos, new Vector3(0, 1, 0), deltaRot.eulerAngles.y);
        transform.RotateAround(nowPos, new Vector3(0, 0, 1), deltaRot.eulerAngles.z);

        //Move
        Vector3 deltaMove = (nowPos - pastPos);

        //Scale
        Vector3 deltaSca = new Vector3((nowSca.x - pastSca.x) / pastSca.x, (nowSca.y - pastSca.y) / pastSca.y, (nowSca.z - pastSca.z) / pastSca.z);
        Vector3 playerDis = Quaternion.Inverse(nowRot) * (transform.position - nowPos);
        deltaSca = nowRot * new Vector3(playerDis.x * deltaSca.x, playerDis.y * deltaSca.y, playerDis.z * deltaSca.z);

        //プレイヤーの位置にMoveとScaleの計算結果（移動量）を加えて、移動
        transform.position += deltaMove + deltaSca;

        //Y軸上方向へは床に押される力で上がるので、ここでは移動させない
        if (transform.position.y - player_pastPos.y > 0)
        {
            transform.position = new Vector3(transform.position.x, player_pastPos.y, transform.position.z);
        }
    }

    public void GroundCheck()
    {
        //プレイヤーの状態に合わせてRayの長さを設定
        float rayDistance = origin.y - footPos - radius;
        if (plat != null)
        {
            rayDistance += groundCheckDistance_onPlat;
        }
        else if (m_rigidbody.velocity.y > 0)
        {
            rayDistance += groundCheckDistance_up;
        }
        else
        {
            rayDistance += groundCheckDistance_down;
        }

        //足元にRayを発射して床を確認
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + origin, radius, Vector3.down, out hit, rayDistance, m_layerMask))
        {
            m_isGround = true;
            if ((plat != null && hit.collider.name == plat.name))
            {
                //Debug.Log("同じ床: " + hit.collider.name);
                return;
            }
            else if (hit.collider.transform.CompareTag("Move") || hit.collider.transform.CompareTag("Center"))
            {
                //Debug.Log("新しい床: " + hit.collider.name);
                plat = hit.collider.gameObject;
                pastRot = plat.transform.rotation;
                pastPos = plat.transform.position;
                pastSca = plat.transform.lossyScale;
                nowSca = plat.transform.lossyScale;

                //親にcenterがいるか調べる
                GameObject p = plat;
                while (p.CompareTag("Move") && p.transform.parent != null)
                {
                    if (p.transform.parent.CompareTag("Center"))
                    {
                        center = p.transform.parent.gameObject;
                        //Debug.Log("center: " + center.name);
                        pastRot = center.transform.rotation;
                        pastPos = center.transform.position;
                        pastSca = center.transform.lossyScale;
                        nowSca = center.transform.lossyScale;
                        return;
                    }
                    else
                    {
                        p = p.transform.parent.gameObject;
                    }
                }
            }
            else
            {
                //Debug.Log("動く床じゃない: " + hit.collider.name);
                plat = null;
                center = null;
            }
        }
        else
        {
            //Debug.Log("空中");
            plat = null;
            center = null;
            m_isGround = false;
        }
    }
}
