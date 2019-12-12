using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPieces
{
    public override Tile[] GetPosibleMovements()
    {
        List<Tile> resul = new List<Tile>();

        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                int auxx = (j % 2==0) ? -1 : 1;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
