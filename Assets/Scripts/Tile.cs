using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public static Vector3 startPos = new Vector3(-2, 0,-9);

    public static float tileSize = 1;

    public static Vector3 Position(int x, int y)
    {
        return startPos + new Vector3(x * tileSize, 0, y * tileSize);
    }

    int _x;
    int _y;

    //true if white
    bool color = false;


    public int x {
        get { return _x; }
        set { _x = value; }
    }

    public int y {
        get { return _y; }
        set { _y = value; }
    }

    public bool isWhite {
        get { return color; }
    }

    public void SetWhite()
    {
        color = true;
        GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
    }

    public void SetBlack()
    {
        color = false;
        GetComponent<MeshRenderer>().material.SetColor("_Color", Color.gray);
    }

    

    public void Start()
    {
        if (isWhite)
        {
            SetWhite();
        }
        else
        {
            SetBlack();
        }
    }

    public void SetPosition(int x, int y)
    {
        _x = x;
        _y = y;
        transform.position = startPos + new Vector3(x * tileSize, 0, y * tileSize);
    }

    public Tile()
    {
        
    }
}
