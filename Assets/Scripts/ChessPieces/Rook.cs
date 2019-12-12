using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPieces
{



    public override Tile[] GetPosibleMovements()
    {
        List<Tile> resul = new List<Tile>();

        for (int i = 0; i < 10; i++)
        {
            Tile t = CheckExistTile(_x + i, _y);
            if (t)
            {
                resul.Add(t);
            }
            t = CheckExistTile(_x - i, _y);
            if (t)
            {
                resul.Add(t);
            }
            t = CheckExistTile(_x, _y + i);
            if (t)
            {
                resul.Add(t);
            }
            t = CheckExistTile(_x, _y - i);
            if (t)
            {
                resul.Add(t);
            }
        }

        return resul.ToArray();
    }
}
