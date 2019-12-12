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
    public enum State { Clear = 0, One = 1, Two = 2, Three = 3};

    public State state = State.Clear;

    public List<ChessPieces> pieces = new List<ChessPieces>();

    private static IAMovement _instance = new IAMovement();

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
                    if (copied0[i].CanThreatInAMov(out mov))
                    {
                        //Execute movement
                        threated0 = true;
                        Debug.Log("MOVE TO THREAT");
                    }
                    else
                    {
                        copied0.RemoveAt(i);
                    }
                }

                if (threated0 == false)
                {
                    //Create another piece of chess, maybe with a probability to measure difficulty, or move random
                    Debug.Log("Create piece or random movement");
                }
                break;
            case State.One:
                //There is one threate, create another one, eliminate that one 
                Debug.Log("One threat");

                bool threated1 = false;
                if (Random.Range(0.0f, 1.0f) <= 0.3f)
                {
                    List<ChessPieces> copied1 = new List<ChessPieces>(pieces);

                    while (!threated1 && copied1.Count != 0)
                    {
                        int i = Random.Range(0, copied1.Count);
                        Movement mov;
                        if (copied1[i].CanThreatInAMov(out mov))
                        {
                            //Execute movement
                            threated1 = true;
                            Debug.Log("MOVE TO THREAT");
                        }
                        else
                        {
                            copied1.RemoveAt(i);
                        }
                    }
                }
                if(!threated1)
                {
                    pieces[Random.Range(0, pieces.Count)].MoveToRandom();
                }
                break;
            case State.Two:
                //There are two threat, eliminate one or move random
                Debug.Log("Two threats");
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
