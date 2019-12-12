using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieces : MonoBehaviour
{
    public int _x, _y;

    public Tile tile;

    TurnManager tm;


    Tile[] ts = new Tile[0];

    List<Tile> threatedTile = new List<Tile>();
    public virtual Tile[] GetPosibleMovements() {
        return new Tile[0];
    }


    const int MAX_DIAG = 8;
    //dir must be {1,1}, {1,-1} {-1,1} {-1,-1}
    public Tile[] GetPosibleMovementsDirection(Vector2 start, Vector2 dir)
    {
        List<Tile> resul = new List<Tile>();
        for(int i = 1; i < MAX_DIAG; i++)
        {
            Tile t = CheckExistTile((int)start.x+(int)dir.x*i,(int)start.y+(int)dir.y*i);
            if (t && !t.piece)
            {

                resul.Add(t);
            }
            else
            {
                break;
            }
        }
        return resul.ToArray();
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

    public virtual bool CanThreatInAMov(out IAMovement.Movement mov)
    {
        mov = new IAMovement.Movement();
        return false;
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
        tm = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TurnManager>();
    }

    public virtual void MoveTo(int x, int y)
    {
        _x = x; _y = y;
        if (tile)
        {
            tile.piece = null;
        }
        tile = CheckExistTile(x, y);
        tile.piece = this;
        tile.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        transform.position = Tile.Position(x, y);
    }

    public Tile CheckExistTile(int x, int y)
    {
        RaycastHit hit;
        
        Ray r = new Ray(Tile.Position(x, y) + new Vector3(0, 10, 0), new Vector3(0, -1, 0));

        if (Physics.Raycast(r, out hit, 100, LayerMask.GetMask("Tile")))
        {
            Tile aux =  hit.transform.gameObject.GetComponent<Tile>();
            if(aux == tile)
            {
                return null;
            }
            else
            {
                return aux;
            }
        }
        else
        {
            return null;
        }
    }

    //Return true if player can be kill in next move
    public bool CanKillPlayerInMove(ref IAMovement.Movement mov)
    {
        mov = new IAMovement.Movement();
        foreach (Tile t in GetPosibleMovements())
        {
            LayerMask lm = LayerMask.GetMask("Player");
            if (Physics.Raycast(t.transform.position + new Vector3(0, 10, 0), -Vector3.up, 100.0f,lm))
            {
                mov.isEmpty = false;
                mov.piece = this;
                mov.tile = t;
                return true;
            }
        }
        return false;
    }

    /*
     * fill a table with movements if the piece could kill the player in the next movement
     * Ex, {true, false, true}, this piece can kill the player if stay in the same tile or if it moves 2 tiles
     * array is iniciated by the function
     */
    public void CanKillPlayerInHisNextMove(ref IAMovement.Movement[] array)
    {
        LayerMask[] lm = new LayerMask[] { LayerMask.GetMask("Player"), LayerMask.GetMask("OneMove"), LayerMask.GetMask("TwoMove") };
        foreach (Tile t in GetPosibleMovements())
        {
            for(int i = 0; i < 3; i++)
            {
                if (array[i].isEmpty)
                {

                    if (Physics.Raycast(t.transform.position + new Vector3(0, 10, 0), -Vector3.up, 100.0f, lm[i]))
                    {
                        array[i].isEmpty = false;
                        array[i].tile = t;
                        array[i].piece = this;
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

    private void Update()
    {
        foreach (Tile t in ts)
        {
            t.UnMarkAsThreat();
        }
        ts = GetPosibleMovements();
        Debug.Log(ts.Length);
        foreach (Tile t in ts)
        {
            t.MarkAsThreat();
        }
    }
}
