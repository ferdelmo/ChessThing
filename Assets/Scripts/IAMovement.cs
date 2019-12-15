using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAMovement
{
    public class Movement
    {
        public ChessPieces piece;
        public Tile tile;
        public bool isEmpty;

        public Movement()
        {
            piece = null;
            tile = null;
            isEmpty = true;
        }
        public Movement(ChessPieces p, Tile t)
        {
            piece = p;
            tile = t;
            isEmpty = false;
        }
    }


    public class TileToThread
    {
        List<int> numbers;

        int numT = 0;

        public TileToThread(List<Movement>[] movs)
        {
            numbers = new List<int>();
            for(int i=0; i< movs.Length; i++)
            {
                if(movs[i].Count == 0)
                {
                    numT++;
                    numbers.Add(i);
                    if (i == 0)
                    {
                        numbers.Add(i);
                    }
                }
            }
            IAMovement.Shuffle<int>(ref numbers);
        }

        public int Get()
        {
            if (numbers.Count > 0 && numT>0)
            {
                int resul = numbers[0];
                numbers.RemoveAt(0);
                numT--;
                return resul;
            }
            else
            {
                return -1;
            }
        }
    }

    public static void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, list.Count);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public int difficult = 2;
    public bool showThreats = true;

    public enum State { Clear = 0, One = 1, Two = 2, Three = 3};

    public State state = State.Clear;

    public List<ChessPieces> pieces = new List<ChessPieces>();

    private static IAMovement _instance = new IAMovement();

    public Player player;

    public int totalMovsPosible = 0;

    bool rook = false;

    ChessPieces lastMoved = null;

    public void Reset()
    {
        _instance = new IAMovement();
    }

    private IAMovement()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        pieces = new List<ChessPieces>();
        state = State.Clear;
    }

    public static IAMovement Instance {
        get {
            return _instance;
        }
    }

    public bool IsPlayerKillable(ref Movement mov)
    {
        mov = new Movement();
        foreach (ChessPieces cp in pieces)
        {
            if (cp.CanKillPlayerInMove(ref mov))
            {
                return true;
            }
        }
        return false;
    }

    public static bool CheckPiece(int x, int y)
    {
        RaycastHit hit;

        Ray r = new Ray(Tile.Position(x, y) + new Vector3(0, 10, 0), new Vector3(0, -1, 0));

        if (Physics.Raycast(r, out hit, 100, LayerMask.GetMask("Pieces")))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int MAX_PIECES = 5;
    public void CreatePiece()
    {
        if(pieces.Count < MAX_PIECES)
        {
            int type = 0;  //0 kngiht, 1 bishop, 2 rook
            float p = Random.Range(0.0f, 1.0f);
            if (p<0.45f){
                type = 0;
            }
            else if(0.45f<= p && p < 0.9f)
            {
                type = 1;
            }
            else
            {
                if (!rook)
                {
                    type = 2;
                    rook = true;
                }
                else
                {
                    type = 0;
                }
            }

            //Calculate pos
            int y = Random.Range(0, 7);
            int x = player.x + 7;

            while (!CheckPiece(x, y))
            {
                y = Random.Range(0, 7);
                x = player.x + 7;
            }

            ChessPieces cp = null;

            switch (type)
            {
                case 0:
                    cp = ((GameObject)GameObject.Instantiate(Resources.Load("Prefab/Knight"))).GetComponent<Knight>();
                    break;
                case 1:
                    cp = ((GameObject)GameObject.Instantiate(Resources.Load("Prefab/Bishop"))).GetComponent<Bishop>();
                    break;
                case 2:
                    cp = ((GameObject)GameObject.Instantiate(Resources.Load("Prefab/Rook"))).GetComponent<Rook>();
                    break;
            }
            if (y == player.y)
            {
                y++;
            }
            cp.transform.position = Tile.Position(x, y);
            cp.MoveToNoAnim(x, y);
        }
        else
        {
            Debug.Log("TOO MUCH PIECES");
        }
    }

    //update state and return an array with the movements that can threat the player
    public List<Movement>[] UpdateState()
    {
        List<Movement>[] movs = new List<Movement>[] { new List<Movement>(), new List<Movement>(), new List<Movement>() };

        foreach (ChessPieces cp in pieces)
        {
            cp.CanKillPlayerInHisNextMove(ref movs);
        }

        int threats = 0;

        foreach (List<Movement> m in movs)
        {
            if (m.Count>0)
            {
                threats++;
            }
        }

        switch (threats)
        {
            case 0:
                state = State.Clear;
                break;
            case 1:
                state = State.One;
                break;
            case 2:
                state = State.Two;
                break;
            case 3:
                state = State.Three;
                break;
        }

        return movs;
    }

    public bool KillPlayer()
    {
        Movement kill = new Movement();
        if (IsPlayerKillable(ref kill))
        {
            //kill player and stop game
            kill.piece.KillPlayer();
            return true;
        }
        return false;
    }

    //return true if player is killed
    public void DecideNextMovement()
    {
        int destroyedPieces = 0;

        List<Movement> movsToExec = new List<Movement>();
        List<ChessPieces> destroyed = new List<ChessPieces>();
        for(int i=0;i<pieces.Count;i++)
        {
            Debug.Log(pieces[i]);
            ChessPieces cp = pieces[i];
            if (cp.ShouldDestroy())
            {
                destroyed.Add(cp);
                ++destroyedPieces;
            }
        }

        foreach(ChessPieces cp in destroyed)
        {
            pieces.Remove(cp);
            if(cp is Rook)
            {
                rook = false;
            }
            cp.UnMarkThreatsTile();
            cp.DestroyPiece();
        }

        if (pieces.Count<MAX_PIECES/2)
        {
            CreatePiece();
        }

        if (pieces.Count == 0)
        {
            CreatePiece();
            CreatePiece();
        }

        List<Movement>[] movs = UpdateState();

        switch (state)
        {
            case State.Clear:
                //There are no threats, create one or move random
                Debug.Log("No threats");
                List<ChessPieces> copied0 = new List<ChessPieces>(pieces);

                bool threated0 = false;
                while (!threated0 && copied0.Count != 0)
                {
                    int i = Random.Range(0, copied0.Count);
                    Movement mov;
                    TileToThread ttt = new TileToThread(movs);
                    int auxx;
                    while (!threated0 && (auxx = ttt.Get())!=-1)
                    {
                        if(copied0[i].CanThreatInAMov(out mov, player.x + auxx, player.y))
                        {
                            //Execute movement
                            threated0 = true;
                            Debug.Log("MOVE TO THREAT");
                            movsToExec.Add(new Movement(copied0[i], mov.tile));

                        }
                    }
                    if(!threated0)
                    {
                        copied0.RemoveAt(i);
                    }
                }

                if (threated0 == false)
                {
                    //Create another piece of chess, maybe with a probability to measure difficulty, or move random
                    Debug.Log("Create piece and random movement");
                    //pieces[Random.Range(0, pieces.Count)].MoveToRandom();
                    CreatePiece();
                }
                break;
            case State.One:
                //There is one threat, create another one, eliminate that one 
                Debug.Log("One threat");

                bool threated1 = false;
                if (Random.Range(0.0f, 1.0f) <= 0.4f*(1+difficult))
                {

                    Movement mov;
                    TileToThread ttt = new TileToThread(movs);
                    int auxx;
                    while (!threated1 && (auxx = ttt.Get()) != -1)
                    {
                        List<ChessPieces> copied1 = new List<ChessPieces>(pieces);

                        while (!threated1 && copied1.Count != 0)
                        {
                            int i = Random.Range(0, copied1.Count);
                            //CHANGE TO FIRST TRY TO THREAT NON THREATED TILES
                            if (copied1[i].CanThreatInAMov(out mov, player.x + auxx, player.y))
                            {
                                //Execute movement
                                threated1 = true;
                                Debug.Log("MOVE TO THREAT");
                                movsToExec.Add(new Movement(copied1[i], mov.tile));

                            }
                            if (!threated1)
                            {
                                copied1.RemoveAt(i);
                            }
                        }
                    }
                }
                if(!threated1 || Random.Range(0.0f,1.0f)<=0.25*difficult)
                {
                    Debug.Log("CREATE A PIECE");
                    //pieces[Random.Range(0, pieces.Count)].MoveToRandom();
                    CreatePiece();
                    if (!threated1)
                    {
                        Movement auxMov = new Movement();
                        pieces[Random.Range(0, pieces.Count)].MoveToRandom(out auxMov);
                        movsToExec.Add(auxMov);
                    }
                }
                break;
            case State.Two:
                //There are two threat, eliminate one or move random
                Debug.Log("Two threats");
                Debug.Log("Move one to reduce threat");
                bool avoided = false;
                //while (!avoided)
                //{
                int infinity = 10;
                int t = Random.Range(1, movs.Length);
                while (!avoided)
                {
                    bool all = true;
                    foreach (Movement m in movs[t])
                    {
                        Movement auxMov = new Movement();
                        all &= m.piece.CanAvoidThreatInAMov(out auxMov);
                        if (all)
                        {
                            movsToExec.Add(auxMov);
                            avoided = true;
                        }
                    }
                    while (movs[t].Count == 0 && !avoided)
                    {
                        t = Random.Range(0, movs.Length);
                    }
                    infinity--;
                    if (infinity < 0)
                    {
                        foreach (Movement m in movs[t])
                        {
                            Movement auxMov;
                            m.piece.MoveToRandom(out auxMov);
                            movsToExec.Add(auxMov);
                        }
                        break;
                    }
                }
                //}

                break;
            case State.Three:
                // TOO MANY THREATS, PLAYER CANT WIN
                Debug.Log("PLAYER IS GOING TO LOSE");

                foreach (Movement m in movs[0])
                {
                    Movement auxMov = new Movement();
                    bool aux = m.piece.CanAvoidThreatInAMov(out auxMov);
                    if (aux)
                    {
                        movsToExec.Add(auxMov);
                        avoided = true;
                    }
                }

                foreach (Movement m in movs[2])
                {
                    Movement auxMov = new Movement();
                    bool aux = m.piece.CanAvoidThreatInAMov(out auxMov);
                    if (aux)
                    {
                        movsToExec.Add(auxMov);
                        avoided = true;
                    }
                }

                break;
        }

        List<Vector2> auxPos = new List<Vector2>();
        foreach (Movement mov in movsToExec)
        {
            auxPos.Add(new Vector2(mov.piece._x, mov.piece._y));
            mov.piece.MoveToNoAnim(mov.tile.x, mov.tile.y);
        }
        movs = UpdateState();
        int i_des = 0;
        foreach (Movement mov in movsToExec)
        {
            mov.piece.MoveToNoAnim((int)auxPos[i_des].x, (int)auxPos[i_des].y);
            i_des++;
        }
        if (state == State.Three)
        {
            movsToExec = new List<Movement>();
            Debug.Log("KILL MY LIFE");
            foreach (Movement m in movs[0])
            {
                Movement auxMov = new Movement();
                bool aux = m.piece.CanAvoidThreatInAMov(out auxMov);
                if (aux)
                {
                    movsToExec.Add(auxMov);
                }
            }

            foreach (Movement m in movs[2])
            {
                Movement auxMov = new Movement();
                bool aux = m.piece.CanAvoidThreatInAMov(out auxMov);
                if (aux)
                {
                    movsToExec.Add(auxMov);
                }
            }
        }  

        IAMovement.Shuffle<ChessPieces>(ref pieces);
        foreach (Movement mov in movsToExec)
        {
            lastMoved = mov.piece;
            mov.piece.MoveTo(mov.tile.x, mov.tile.y);
        }
    }
}
