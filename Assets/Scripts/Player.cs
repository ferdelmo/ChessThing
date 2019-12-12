using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    int _x, _y;

    public int x {
        get { return _x; }
    }

    public int y {
        get { return _y; }
    }


    public void SetPosition(int x, int y)
    {
        _x = x; _y = y;
        transform.position = Tile.Position(x, y);
    }

    public void Start()
    {
        SetPosition(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetPosition(_x + 1, _y);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetPosition(_x, _y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetPosition(_x - 1, _y);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetPosition(_x, _y - 1);
        }
    }
}
