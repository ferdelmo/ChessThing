﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public static Vector3 startPos = new Vector3(-2, 0,-9);

    public static float tileSize = 1;

    public ChessPieces piece=null;

    public static Vector3 Position(int x, int y)
    {
        return startPos + new Vector3(x * tileSize, 0, y * tileSize);
    }

    public int _x;
    public int _y;

    public GameObject feedback;

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

    public void MoveTo(int x, int y)
    {
        _x = x;
        _y = y;
        StartCoroutine(SmoothOut(x, y));
    }

    float SmoothStart(float t)
    {
        return t * t;
    }

    static float AnimDur = .25f;
    //Let the tile fall and then move it
    IEnumerator SmoothOut(int x, int y)
    {
        Vector3 startPos = transform.position;

        float counter = 0;

        while (counter < AnimDur)
        {
            counter += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, -5 * SmoothStart(counter / AnimDur), transform.position.z);
            yield return null;
        }
        transform.position = Tile.Position(x, y);
    }

    public void MoveToNewPosition(int x, int y)
    {
        //Add some animations to look coler if time
        SetPosition(x, y);
    }

    public void MarkAsThreat()
    {
        //GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        feedback.SetActive(true);
    }

    public void UnMarkAsThreat()
    {
        /*
        if (isWhite)
        {
            SetWhite();
        }
        else
        {
            SetBlack();
        }
        */
        feedback.SetActive(false);
    }

    public static bool operator ==(Tile lhs, Tile rhs)
    {
        if (lhs != null && rhs != null && lhs._x == rhs._x && lhs._y == rhs._y)
        {
            return true;
        }
        else if(!lhs && !rhs)
        {
            return true;
        }
        return false;
    }
    public static bool operator !=(Tile lhs, Tile rhs)
    {
        if (!lhs && !rhs)
        {
            return false;
        }
        if (!lhs)
        {
            return true;
        }
        if (!rhs)
        {
            return true;
        }
        if (lhs._x == rhs._x && lhs._y == rhs._y)
        {

            return false;
        }
        return true;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Tile objAsPart = obj as Tile;
        if (objAsPart == null) return false;
        else return this == (Tile)obj;
    }
}
