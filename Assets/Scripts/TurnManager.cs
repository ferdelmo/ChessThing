using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public bool player_turn = true;

    public float player_turn_time = 5.0f;
    public float machine_turn_time = 2.5f;

    ChessPieces cp;

    public MyText playerTime;
    public MyText machineTime;


    public bool IsPlayerTurn {
        get { return player_turn; }
    }

    float pTimer;
    float mTimer;

    // Start is called before the first frame update
    void Start()
    {
        ChessPieces[] cps = GameObject.FindObjectsOfType<ChessPieces>();
        foreach(ChessPieces cp in cps)
        {
            IAMovement.Instance.pieces.Add(cp);
        }

        pTimer = player_turn_time;
        mTimer = machine_turn_time;
        playerTime.Appear();
    }

    // Update is called once per frame
    void Update()
    {
        //Inform about time
        if (player_turn)
        {
            playerTime.SetText=(player_turn_time-pTimer).ToString("0.00");
            machineTime.SetText = (machine_turn_time).ToString("0.00");
            pTimer += Time.deltaTime;
            mTimer = 0;
        }
        else
        {
            machineTime.SetText = (machine_turn_time - mTimer).ToString("0.00");
            playerTime.SetText = (player_turn_time).ToString("0.00");
            mTimer += Time.deltaTime;
            pTimer = 0;
        }
    }

    public void AdvanceTurn()
    {
        StopAllCoroutines();
        player_turn = !player_turn;
        StartCoroutine(DelayTurn());

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
            
            yield return new WaitForSeconds(machine_turn_time);

            foreach (ChessPieces mark in IAMovement.Instance.pieces)
            {
                mark.UnMarkThreatsTile();
            }
            if (IAMovement.Instance.DecideNextMovement())
            {
                Debug.Log("END GAMEEE");
            }
            else
            {
                AdvanceTurn();
            }
            foreach (ChessPieces mark in IAMovement.Instance.pieces)
            {
                mark.MarkThreatsTile();
            }
        }
        
    }
}
