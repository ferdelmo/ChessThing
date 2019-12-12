using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPieces
{

    public override Tile[] GetPosibleMovements()
    {
        List<Tile> resul = new List<Tile>();
        for(int j = 0; j < 4; j++)
        {
            int auxx = (j % 2==0) ? -1 : 1;
            int auxy = (j / 2 == 0) ? -1 : 1;
            Debug.Log(auxx + " " + auxy);
            Tile[] movs = GetPosibleMovementsDirection(new Vector2(_x,_y), new Vector2(auxx, auxy));

            resul.AddRange(movs);
        }

        return resul.ToArray();
    }
}
