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
    }


    public class TileToThread
    {
        List<int> numbers = new List<int> { 0, 1, 2 };

        public int Get()
        {
            if (numbers.Count > 0)
            {
                int aux = Random.Range(0, numbers.Count);
                int resul = numbers[aux];
                numbers.RemoveAt(aux);
                return resul;
            }
            else
            {
                return -1;
            }
        }
    }

    const int MAX_PIECES = 5;

    public enum State { Clear = 0, One = 1, Two = 2, Three = 3};

    public State state = State.Clear;

    public List<ChessPieces> pieces = new List<ChessPieces>();

    private static IAMovement _instance = new IAMovement();

    public Player player;

    private IAMovement()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public static IAMovement Instance {
        get {
            return _instance;
        }
    }

    public bool CanKillPlayer(ref Movement mov)
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

    public void CreatePiece()
    {
        if(pieces.Count < MAX_PIECES)
        {
            int type = Random.Range(0, 3); //0 kngiht, 1 bishop, 2 rook

            //Calculate pos
            int y = Random.Range(0, 7);
            int x = player.x + 7;
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
            cp.MoveTo(x, y);
            pieces.Add(cp);
        }
        else
        {
            Debug.Log("TOO MUCH PIECES");
        }
    }

    //update state and return an array with the movements that can threat the player
    public Movement[] UpdateState()
    {
        Movement[] movs = new Movement[] { new Movement(), new Movement(), new Movement() };

        foreach (ChessPieces cp in pieces)
        {
            cp.CanKillPlayerInHisNextMove(ref movs);
        }

        int threats = 0;

        List<Movement> finalMovs = new List<Movement>();

        foreach (Movement m in movs)
        {
            if (!m.isEmpty)
            {
                threats++;
                finalMovs.Add(m);
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

        return finalMovs.ToArray();
    }

    //return true if player is killed
    public bool DecideNextMovement()
    {
        for(int i=0;i<pieces.Count;i++)
        {
            ChessPieces cp = pieces[i];
            if (cp.ShouldDestroy())
            {
                pieces.RemoveAt(i);
                GameObject.Destroy(cp.gameObject);
            }
        }
        Movement kill = new Movement();
        if (CanKillPlayer(ref kill))
        {
            //kill player and stop game
            return true;
        }
        Movement[] movs = UpdateState();

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
                    TileToThread ttt = new TileToThread();
                    int auxx;
                    while (!threated0 && (auxx = ttt.Get())!=-1)
                    {
                        if(copied0[i].CanThreatInAMov(out mov, player.x + auxx, player.y))
                        {
                            //Execute movement
                            threated0 = true;
                            Debug.Log("MOVE TO THREAT");
                            copied0[i].MoveTo(mov.tile.x, mov.tile.y);

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
                //There is one threate, create another one, eliminate that one 
                Debug.Log("One threat");

                bool threated1 = false;
                if (Random.Range(0.0f, 1.0f) <= 0.75f)
                {
                    List<ChessPieces> copied1 = new List<ChessPieces>(pieces);

                    while (!threated1 && copied1.Count != 0)
                    {
                        int i = Random.Range(0, copied1.Count);
                        Movement mov;
                        TileToThread ttt = new TileToThread();
                        int auxx;
                        //CHANGE TO FIRST TRY TO THREAT NON THREATED TILES
                        while (!threated1 && (auxx = ttt.Get()) != -1)
                        {
                            if (copied1[i].CanThreatInAMov(out mov, player.x + auxx, player.y))
                            {
                                //Execute movement
                                threated1 = true;
                                Debug.Log("MOVE TO THREAT");
                                copied1[i].MoveTo(mov.tile.x, mov.tile.y);

                            }
                        }
                        if (!threated1)
                        {
                            copied1.RemoveAt(i);
                        }
                    }
                }
                if(!threated1)
                {
                    Debug.Log("CREATE A PIECE");
                    //pieces[Random.Range(0, pieces.Count)].MoveToRandom();
                    CreatePiece();
                }
                break;
            case State.Two:
                //There are two threat, eliminate one or move random
                Debug.Log("Two threats");
                Debug.Log("Move one to reduce threat");
                movs[Random.Range(0, movs.Length)].piece.MoveToRandom();
                break;
            case State.Three:
                // TOO MANY THREATS, PLAYER CANT WIN
                Debug.Log("PLAYER IS GOING TO LOSE");

                break;
        }

        return false;
    }
}
