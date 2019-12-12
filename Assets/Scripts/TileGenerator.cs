using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{

    // Y has to be a even number
    public int x, y;

    public GameObject g_Tile;

    Tile[,] tiles;

    int actual = 0;

    int lastX;

    // Start is called before the first frame update
    void Start()
    {
        lastX = x;

        tiles = new Tile[x,y];

        bool white = false;

        for(int i=0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                tiles[i, j] = Instantiate(g_Tile).GetComponent<Tile>();
                Tile aux = tiles[i, j];
                aux.transform.SetParent(transform);
                aux.SetPosition(i, j);
                if (white)
                {
                    aux.SetWhite();
                }
                else
                {
                    aux.SetBlack();
                }
                if (j < y - 1)
                {
                    white = !white;
                }
            }
        }
    }

    //Destroy the last line, and create one in front
    public void NewLine()
    {
        for(int i = 0; i < y; i++)
        {
            tiles[actual, i].SetPosition(lastX, i);
        }
        lastX++;

        actual = (actual + 1) % x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
