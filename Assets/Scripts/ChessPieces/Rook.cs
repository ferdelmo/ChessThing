using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPieces
{



    public override Tile[] GetPosibleMovements()
    {
        List<Tile> resul = new List<Tile>();

        //X movement
        Tile[] aux = GetPosibleMovementsDirection(new Vector2(_x, _y), new Vector2(1, 0));
        resul.AddRange(aux);

        aux = GetPosibleMovementsDirection(new Vector2(_x, _y), new Vector2(-1, 0));
        resul.AddRange(aux);

        //Y movement
        aux = GetPosibleMovementsDirection(new Vector2(_x, _y), new Vector2(0, 1));
        resul.AddRange(aux);

        aux = GetPosibleMovementsDirection(new Vector2(_x, _y), new Vector2(0, -1));
        resul.AddRange(aux);
        return resul.ToArray();
    }
    public override bool CanThreatInAMov(out IAMovement.Movement mov, int x, int y)
    {
        mov = new IAMovement.Movement();
        if(_y != player.y)
        {
            Tile t = CheckExistTile(x, _y);
            if(t && !t.piece)
            {
                mov.piece = this;
                mov.tile = t;
                mov.isEmpty = false;
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
