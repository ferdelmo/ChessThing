using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieces : MonoBehaviour
{
    public int _x, _y;

    public Tile tile;

    TurnManager tm;

    protected Player player;

    Tile[] ts = new Tile[0];

    List<Tile> threatedTile = new List<Tile>();

    MeshCollider[] meshCollider;

    bool showThreats = false;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        meshCollider = GetComponentsInChildren<MeshCollider>();
        foreach(MeshCollider mc in meshCollider)
        {
            mc.enabled = false;
        }


        IAMovement.Instance.pieces.Add(this);
        showThreats = IAMovement.Instance.showThreats;

        tm = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TurnManager>();
    }

    public virtual Tile[] GetPosibleMovements()
    {
        return new Tile[0];
    }

    public virtual Tile[] GetPosibleMovementsNoPlayerColumn()
    {
        List<Tile> lt = new List<Tile>();
        lt.AddRange(GetPosibleMovements());
        for (int i=0;i<lt.Count;i++)
        {
            Tile t = lt[i];
            if (t.y == player.y)
            {
                lt.RemoveAt(i); ;
            }
        }

        return lt.ToArray();
    }

    const int MAX_DIAG = 8;
    //dir must be {1,1}, {1,-1} {-1,1} {-1,-1}
    public Tile[] GetPosibleMovementsDirection(Vector2 start, Vector2 dir)
    {
        List<Tile> resul = new List<Tile>();
        for (int i = 1; i < MAX_DIAG; i++)
        {
            Tile t = CheckExistTile((int)start.x + (int)dir.x * i, (int)start.y + (int)dir.y * i);
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
        foreach (Tile t in posibleMovs)
        {
            threatedTile.Add(t);
            t.MarkAsThreat();
        }
    }

    //Can threat the (x,y) tile in a mov, and return the mov that does it
    public virtual bool CanThreatInAMov(out IAMovement.Movement mov, int x, int y)
    {
        mov = new IAMovement.Movement();
        Tile[] tiles = GetPosibleMovementsNoPlayerColumn();

        foreach (Tile t in tiles)
        {
            if (CanGoToFrom(t.x, t.y, x, y))
            {
                if (!t.piece)
                {
                    mov.isEmpty = false;
                    mov.piece = this;
                    mov.tile = t;
                    return true;
                }
            }
        }

        return false;
    }

    //Avoid to cover position x,y
    public virtual bool AvoidThreat(out IAMovement.Movement mov, int x, int y)
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

    public virtual void MoveToNoAnim(int x, int y)
    {
        _x = x; _y = y;
        if (tile)
        {
            tile.piece = null;
        }
        tile = CheckExistTile(x, y);
        if (tile)
        {
            tile.piece = this;
        }
        transform.position = Tile.Position(x, y);
    }

    public virtual void MoveTo(int x, int y)
    {
        _x = x; _y = y;
        if (tile)
        {
            tile.piece = null;
        }
        tile = CheckExistTile(x, y);
        if (tile)
        {
            tile.piece = this;
        }
        StartCoroutine(MoveToAnim(tile));
    }


    public static float Arch(float t)
    {
        return t * (1 - t);
    }

    public static float AnimDur = .75f;
    public IEnumerator MoveToAnim(Tile t)
    {

        Vector3 startPos = transform.position;

        float counter = 0;

        while (counter < AnimDur)
        {
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, t.transform.position, counter / AnimDur);
            transform.position = new Vector3(transform.position.x, 1 * Arch(counter / AnimDur), transform.position.z);
            yield return null;
        }
        transform.position = Tile.Position(t.x, t.y);
    }

    public Tile CheckExistTile(int x, int y)
    {
        RaycastHit hit;

        Ray r = new Ray(Tile.Position(x, y) + new Vector3(0, 10, 0), new Vector3(0, -1, 0));

        if (Physics.Raycast(r, out hit, 100, LayerMask.GetMask("Tile")))
        {
            Tile aux = hit.transform.gameObject.GetComponent<Tile>();
            if (aux == tile)
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
            if (Physics.Raycast(t.transform.position + new Vector3(0, 10, 0), -Vector3.up, 100.0f, lm))
            {
                mov.isEmpty = false;
                mov.piece = this;
                mov.tile = t;
                return true;
            }
        }
        return false;
    }

    public bool CanGoToFrom(int x, int y, int destx, int desty)
    {
        int auxx = _x, auxy = _y;
        _x = x; _y = y;
        Tile[] tiles = GetPosibleMovements();
        _x = auxx; _y = auxy;
        foreach (Tile t in tiles)
        {

            if (t.x == destx && t.y == desty && t.x != x && t.y != y)
            {
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
            for (int i = 0; i < 3; i++)
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

    public bool ShouldDestroy()
    {
        if (tile)
        {
            return !(tile.piece != null);
        }
        else
        {
            return true;
        }
    }

    public virtual void MoveToRandom()
    {
        Tile[] posibleMovs = GetPosibleMovementsNoPlayerColumn();

        Tile dest = posibleMovs[Random.Range(0, posibleMovs.Length)];

        MoveTo(dest.x, dest.y);
    }

    private void Update()
    {
        if (showThreats)
        {
            foreach (Tile t in ts)
            {
                t.UnMarkAsThreat();
            }
            ts = GetPosibleMovements();
            foreach (Tile t in ts)
            {
                t.MarkAsThreat();
            }
        }
    }

    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCoroutine());
    }

    public IEnumerator KillPlayerCoroutine()
    {
        player.PrepareToKill();

        GetComponent<BoxCollider>().enabled = false;
        foreach (MeshCollider mc in meshCollider)
        {
            mc.enabled = true;

        }
        if (AnimDur > 0.4)
        {
            AnimDur = 0.4f;
        }
        MoveTo(player.x, player.y);

        yield return new WaitForSeconds(2);

        Rigidbody rb = player.pawn.GetComponent<Rigidbody>();


        Camera.main.transform.parent.GetComponent<MyCamera>().ShowGameOver();
    }
}

