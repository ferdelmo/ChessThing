using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{

    public int x, y;

    public GameObject g_Tile;

    Tile[,] tiles;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new Tile[x,y];

        bool white = false;

        for(int i=0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                tiles[i, j] = Instantiate(g_Tile).GetComponent<Tile>();
                Tile aux = tiles[i, j];
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
