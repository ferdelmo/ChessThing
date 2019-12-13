using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Player follow;

    public int x_offset, y_offset, h_offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {

        transform.position = follow.transform.position + new Vector3(x_offset, h_offset, y_offset);
        transform.position = new Vector3(transform.position.x, h_offset, transform.position.z);
    }
}
