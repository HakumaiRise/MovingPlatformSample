using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    Rigidbody m_rigidbody;
    Platforming m_platforming;

    void Start()
    {
        m_rigidbody = transform.GetComponent<Rigidbody>();
        m_platforming = transform.GetComponent<Platforming>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, 0)), Time.deltaTime * 1200);
            m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, 3);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 270, 0)), Time.deltaTime * 1200);
            m_rigidbody.velocity = new Vector3(-3, m_rigidbody.velocity.y, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 180, 0)), Time.deltaTime * 1200);
            m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, -3);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 90, 0)), Time.deltaTime * 1200);
            m_rigidbody.velocity = new Vector3(3, m_rigidbody.velocity.y, 0);
        }
        else
        {
            m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, 0);
        }

        if (Input.GetKey(KeyCode.Space) && m_platforming.m_isGround)
        {
            m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 5, m_rigidbody.velocity.z);
            m_platforming.plat = null;
            m_platforming.center = null;
        }

        if(transform.position.y < -5)
        {
            transform.position = new Vector3(0, 0.5f, 0);
        }
    }
}
