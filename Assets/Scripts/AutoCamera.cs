using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float distance;
    float maxY;
    float minY;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        maxY = player.transform.position.y + 1.5f;
        minY = player.transform.position.y - 0.5f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (maxY < player.transform.position.y)
        {
            transform.position = transform.position + new Vector3(0, player.transform.position.y - maxY, 0);
            maxY = player.transform.position.y;
            minY = maxY - 2;
        }
        if (minY > player.transform.position.y)
        {
            transform.position = transform.position + new Vector3(0, player.transform.position.y - minY, 0);
            minY = player.transform.position.y;
            maxY = minY + 2;
        }

        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z - distance);
    }
}
