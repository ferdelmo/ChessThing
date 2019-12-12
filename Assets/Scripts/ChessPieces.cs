using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieces : MonoBehaviour
{
    public int _x, _y;

    TurnManager tm;

    List<Tile> threatedTile = new List<Tile>();
    public virtual Tile[] GetPosibleMovements() {
        return new Tile[0];
    }

    public virtual void MarkThreatsTile()
    {
        Tile[] posibleMovs = GetPosibleMovements();
        threatedTile.Clear();
        foreach(Tile t in posibleMovs)
        {
            threatedTile.Add(t);
            t.MarkAsThreat();
        }
    }
    public virtual void UnMarkThreatsTile()
    {
        foreach (Tile t in threatedTile)
        {
            t.UnMarkAsThreat();
        }
    }

    public virtual void Start()
    {
        MoveTo(_x,_y);
        tm = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TurnManager>();
    }

    public virtual void MoveTo(int x, int y)
    {
        _x = x; _y = y;
        transform.position = Tile.Position(x, y);
    }

    public Tile CheckExistTile(int x, int y)
    {
        RaycastHit hit;

        LayerMask lm = LayerMask.GetMask("Pieces", "Tile");

        if (Physics.Raycast(Tile.Position(x, y) + new Vector3(0, 10, 0), -Vector3.up, out hit, 100.0f,lm))
        {
            return hit.transform.gameObject.GetComponent<Tile>();
        }
        else
        {
            return null;
        }
    }

    //Return true if player can be kill in next move
    public bool CanKillPlayerInMove()
    {
        foreach (Tile t in GetPosibleMovements())
        {
            LayerMask lm = LayerMask.GetMask("Player");

            if (Physics.Raycast(t.transform.position + new Vector3(0, 10, 0), -Vector3.up, 100.0f,lm))
            {
                return true;
            }
        }
        return false;
    }

    /*
     * fill a table with trues or false if the piece could kill the player in the next movement
     * Ex, {true, false, true}, this piece can kill the player if stay in the same tile or if it moves 2 tiles
     * array is iniciated by the function
     */
    public void CanKillPlayerInHisNextMove(ref bool[] array)
    {
        LayerMask[] lm = new LayerMask[] { LayerMask.GetMask("Player"), LayerMask.GetMask("OneMove"), LayerMask.GetMask("TwoMove") };
        foreach (Tile t in GetPosibleMovements())
        {
            for(int i = 0; i < 3; i++)
            {
                if (!array[i])
                {

                    if (Physics.Raycast(t.transform.position + new Vector3(0, 10, 0), -Vector3.up, 100.0f, lm[i]))
                    {
                        array[i] = true;
                    }
                }
            }
        }
    }

    public virtual void MoveToRandom()
    {
        Tile[] posibleMovs = GetPosibleMovements();

        Tile dest = posibleMovs[Random.Range(0, posibleMovs.Length)];

        MoveTo(dest.x, dest.y);
    }
}
