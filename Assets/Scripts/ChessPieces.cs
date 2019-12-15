using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ChessPieces : MonoBehaviour
{
    public int _x, _y;

    public Tile tile;

    TurnManager tm;
    TileGenerator tg;

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
        tg = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TileGenerator>();
    }

    public virtual Tile[] GetPosibleMovements()
    {
        return new Tile[0];
    }

    public virtual Tile[] GetPosibleMovementsNoPlayerColumn()
    {
        List<Tile> lt = new List<Tile>();
        List<Tile> resul = new List<Tile>(GetPosibleMovements());
        for (int i=0;i<resul.Count;i++)
        {
            Tile t = resul[i];
            if (t.y == player.y)
            {
                lt.Add(t);
            }
        }
        foreach(Tile t in lt)
        {
            resul.Remove(t);
        }

        IAMovement.Shuffle<Tile>(ref resul);

        return resul.ToArray();
    }

    const int MAX_DIAG = 8;
    //dir must be {1,1}, {1,-1} {-1,1} {-1,-1}
    public Tile[] GetPosibleMovementsDirection(Vector2 start, Vector2 dir)
    {
        List<Tile> resul = new List<Tile>();
        for (int i = 1; i < MAX_DIAG; i++)
        {
            Tile t = CheckExistTile((int)start.x + (int)dir.x * i, (int)start.y + (int)dir.y * i);
            if (t && IAMovement.CheckPiece(t.x,t.y))
            {

                resul.Add(t);
            }
            else
            {
                i=MAX_DIAG;
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

    public virtual bool CanAvoidThreatInAMov(out IAMovement.Movement mov)
    {
        mov = new IAMovement.Movement();

        Tile[] tiles = GetPosibleMovementsNoPlayerColumn();
        int aux_x = _x, aux_y = _y;

        bool can = false;

        foreach(Tile t in tiles)
        {
            bool valid = false;

            _x = t.x; _y = t.y;
            for(int i = 0; i < 3; i++)
            {
                valid |= CanGoToFrom(t.x, t.y, player.x + i,player.y);
            }
            if (!valid)
            {
                mov.isEmpty = false;
                mov.piece = this;
                mov.tile = t;
                can = true;
                break;
            }
        }

        _x = aux_x; _y = aux_y;
        return can;


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
        else
        {
            Debug.Log("ESTO HACE QUE REVIENTE MUUUUUUY FUERTE");
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
        else
        {
            Debug.Log("ESTO HACE QUE REVIENTE MUUUUUUY FUERTE");
        }
        StartCoroutine(MoveToAnim(x,y));
    }


    public static float Arch(float t)
    {
        return t * (1 - t);
    }

    public static float AnimDur = .75f;
    public IEnumerator MoveToAnim(int x, int y)
    {

        Vector3 startPos = transform.position;

        float counter = 0;

        Vector3 pos = Tile.Position(x, y); ;

        while (counter < AnimDur)
        {
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, pos, counter / AnimDur);
            transform.position = new Vector3(transform.position.x, 1 * Arch(counter / AnimDur), transform.position.z);
            yield return null;
        }
        transform.position = pos;
    }

    public Tile CheckExistTile(int x, int y)
    {
        return CheckExistTileNoRay(x, y);
        RaycastHit hit;

        Ray r = new Ray(Tile.Position(x, y) + new Vector3(0, 10, 0), new Vector3(0, -1, 0));

        //Ray r = new Ray(new Vector3(x, 10, y), new Vector3(0, -1, 0));

        if (Physics.Raycast(r, out hit, 100, LayerMask.GetMask("Tile")))
        {
            Tile aux = hit.transform.gameObject.GetComponent<Tile>();
            if (tile && aux == tile)
            {
                return null;
            }
            else if(aux)
            {
                return aux;
            }
            else
            {
                return CheckExistTileNoRay(x, y);
            }
        }
        else
        {
            return CheckExistTileNoRay(x, y);
        }
    }

    public Tile CheckExistTileNoRay(int x, int y)
    {
        Tile resul = null;
        Tile[,] tiles = tg.Map;

        for(int i = 0; i < tiles.GetLength(0) && resul == null; i++)
        {
            if (tiles[i,0].x == x)
            {
                for(int j = 0; j < tiles.GetLength(1) && resul == null; j++)
                {
                    if (tiles[i, j].y == y)
                    {
                        resul = tiles[i,j];
                    }
                }
            }
        }
        return resul;

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
    public void CanKillPlayerInHisNextMove(ref List<IAMovement.Movement>[] array)
    {
        LayerMask[] lm = new LayerMask[] { LayerMask.GetMask("Player"), LayerMask.GetMask("OneMove"), LayerMask.GetMask("TwoMove") };

        foreach (Tile t in GetPosibleMovements())
        {
            for (int i = 0; i < 3; i++)
            {
                if (Physics.Raycast(t.transform.position + new Vector3(0, 10, 0), -Vector3.up, 100.0f, lm[i]))
                {
                    IAMovement.Movement aux = new IAMovement.Movement();
                    aux.isEmpty = false;
                    aux.tile = t;
                    aux.piece = this;
                    array[i].Add(aux);
                }
            }
        }
    }

    public bool ShouldDestroy()
    {
        if (!tile)
        {
            tile = CheckExistTile(_x, _y);
            if (tile)
            {
                tile.piece = this;
            }
        }
        if (tile)
        {
            if (!tile.piece)
            {
                tile = CheckExistTile(_x, _y);
                if (tile)
                {
                    tile.piece = this;
                }
                else
                {
                    return true;
                }
            }
            return !(tile.piece != null);
        }
        else
        {
            return true;
        }
    }

    public virtual void MoveToRandom(out IAMovement.Movement mov)
    {
        Tile[] posibleMovs = GetPosibleMovementsNoPlayerColumn();

        Tile dest = posibleMovs[Random.Range(0, posibleMovs.Length)];

        mov = new IAMovement.Movement();
        mov.isEmpty = false;
        mov.tile = dest;
        mov.piece = this;
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

        float dist = Vector2.Distance(new Vector2(_x, _y), new Vector2(player.x, player.y));
        AnimDur = 0.15f;
        if (dist > 1)
        {
            AnimDur += dist * 0.025f;
        }
        MoveTo(player.x, player.y);

        yield return new WaitForSeconds(2);

        Rigidbody rb = player.pawn.GetComponent<Rigidbody>();


        Camera.main.transform.parent.GetComponent<MyCamera>().ShowGameOver();
    }

    public void DestroyPiece()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
        //StartCoroutine(Reduce());
    }

    IEnumerator Reduce()
    {
        float anim = 0.5f;
        float counter = 0;

        while (counter < anim)
        {
            counter += Time.deltaTime;
            transform.localScale -= transform.localScale / anim * Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        ChessPieces objAsPart = obj as ChessPieces;
        if (objAsPart == null) return false;
        else return this == (ChessPieces)obj;
    }

    public static bool operator ==(ChessPieces lhs, ChessPieces rhs)
    {
        if (lhs != null && rhs != null && lhs._x == rhs._x && lhs._y == rhs._y)
        {
            return true;
        }
        else if(!lhs && !rhs)
        {
            return true;
        }
        return false;
    }
    public static bool operator !=(ChessPieces lhs, ChessPieces rhs)
    {
        if (!lhs && !rhs)
        {
            return false;
        }
        if (!lhs)
        {
            return true;
        }
        if (!rhs)
        {
            return true;
        }
        if (lhs._x == rhs._x && lhs._y == rhs._y)
        {

            return false;
        }
        return true;
    }
}

