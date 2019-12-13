using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{

    // Y has to be a even number
    public int x, y;

    public GameObject g_Tile;

    public ChessPieces[] pieces;

    Tile[,] tiles;

    public Tile[,] Map {
        get { return tiles; }
    }

    int actual = 0;

    int lastX;

    bool initPieces = true;

    // Start is called before the first frame update
    void Awake()
    {
        pieces = GameObject.FindObjectsOfType<ChessPieces>();
        lastX = x;

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

    private void Start()
    {
        
    }
    //Destroy the last line, and create one in front
    public void NewLine()
    {
        for(int i = 0; i < y; i++)
        {
            Tile t = tiles[actual, i];
            t.MoveTo(lastX, i);
            if (t.isWhite) {
                t.SetWhite();
            }
            else
            {
                t.SetBlack();
            }
            t.piece = null;
        }
        lastX++;

        actual = (actual + 1) % x;
    }

    // Update is called once per frame
    void Update()
    {
        if (initPieces)
        {
            foreach (ChessPieces cp in pieces)
            {
                cp.MoveTo(cp._x, cp._y);
            }
            initPieces = false;
        }
        else
        {

        }
    }
}
