using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int start_x, start_y;

    int _x, _y;

    TileGenerator tg;
    TurnManager tm;

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
        SetPosition(start_x, start_y);

        tg = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TileGenerator>();
        tm = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tm.IsPlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SetPosition(_x + 1, _y);
                tg.NewLine();
                tm.AdvanceTurn();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                SetPosition(_x + 2, _y);
                tg.NewLine();
                tg.NewLine();
                tm.AdvanceTurn();
            }
        }
    }
}
