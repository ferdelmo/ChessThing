using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Player follow;

    public int x_offset, y_offset, h_offset;

    int _x, _y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _x = follow.x + x_offset;
        _y = follow.y + y_offset;

        transform.position = Tile.Position(_x, _y) + new Vector3(0, h_offset, 0);
    }
}
