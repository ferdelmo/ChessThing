using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public bool player_turn = true;

    public float player_turn_time = 5.0f;
    public float machine_turn_time = 2.5f;

    public ChessPieces[] pieces;

    ChessPieces cp;

    public bool IsPlayerTurn {
        get { return player_turn; }
    }
    // Start is called before the first frame update
    void Start()
    {
        pieces = GameObject.FindObjectsOfType<ChessPieces>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool NoValidMove()
    {
        bool[] movs = { false, false, false };

        foreach(ChessPieces cp in pieces)
        {
            cp.CanKillPlayerInHisNextMove(ref movs);
            if(movs[0] && movs[1] && movs[2])
            {
                //NO VALID MOVES
                return true;
            }
        }

        //There are valid movs
        return false;
    }

    public void AdvanceTurn()
    {
        StopAllCoroutines();
        player_turn = !player_turn;
        StartCoroutine(DelayTurn());

    }
    
    //AQUI ESTA LA MANDANGA
    public void DecideNextMove()
    {
        //Machine makes movement
        if (!cp.CanKillPlayerInMove())
        {
            cp.MoveToRandom();
            cp.MarkThreatsTile();


            AdvanceTurn();
        }
        else
        {
            Debug.Log("END GAME OR KILL PLAYER OR ONE LIFE LESS");
        }


        if (NoValidMove())
        {
            Debug.Log("MEGA FAIL, NO VALID MOV FOR THE PLAYER");
        }
    }

    IEnumerator DelayTurn()
    {
        if (player_turn)
        {
            yield return new WaitForSeconds(player_turn_time);
            AdvanceTurn();
        }
        else
        {
            ChessPieces aux = cp;
            
            cp = pieces[Random.Range(0, pieces.Length)];
            yield return new WaitForSeconds(machine_turn_time);
            if (aux)
            {
                aux.UnMarkThreatsTile();
            }
            DecideNextMove();
        }
        
    }
}
