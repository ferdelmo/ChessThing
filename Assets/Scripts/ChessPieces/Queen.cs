using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPieces
{
    public Tile[] GetPosibleMovementsRook()
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

    public Tile[] GetPosibleMovementsBishop()
    {
        List<Tile> resul = new List<Tile>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int auxx = (j % 2 == 0) ? -1 : 1;
                int auxy = (j / 2 == 0) ? -1 : 1;
                Tile t = CheckExistTile(i * auxx + _x, i * auxy + _y);
                if (t)
                {
                    resul.Add(t);
                }
            }
        }

        return resul.ToArray();
    }

    public override Tile[] GetPosibleMovements()
    {
        Tile[] rook = GetPosibleMovementsRook();
        Tile[] bishop = GetPosibleMovementsBishop();
        Tile[] resul = new Tile[rook.Length + bishop.Length];

        rook.CopyTo(resul, 0);
        bishop.CopyTo(resul, rook.Length);
        return resul;
    }
}
