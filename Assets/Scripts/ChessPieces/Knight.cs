using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPieces
{


    int[] X = { 2, 1, -1, -2, -2, -1, 1, 2 };
    int[] Y = { 1, 2, 2, 1, -1, -2, -2, -1 };

    public override Tile[] GetPosibleMovements()
    {
        List<Tile> resul = new List<Tile>();

        for(int i = 0; i < 8; i++)
        {
            Tile t = CheckExistTile(_x + X[i], _y + Y[i]);
            if (t)
            {
                
                resul.Add(t);
            }
        }

        return resul.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
