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

    public MyCamera camera;


    public int turns_survived = 0;

    public bool IsPlayerTurn {
        get { return player_turn; }
    }

    float pTimer;
    float mTimer;

    // Start is called before the first frame update
    void Start()
    {
        switch (IAMovement.Instance.difficult)
        {
            case 0:
                player_turn_time = 4.0f;
                machine_turn_time = 1f;
                ChessPieces.AnimDur = .75f;
                break;
            case 1:
                player_turn_time = 3f;
                machine_turn_time = 0.5f;
                ChessPieces.AnimDur = .35f;
                break;
            case 2:
                player_turn_time = 1f;
                machine_turn_time = 0.25f;
                ChessPieces.AnimDur = .25f;
                break;
        }
        pTimer = player_turn_time;
        mTimer = machine_turn_time;

        camera.startRot_m = camera.startRot_p * machine_turn_time / player_turn_time;
    }

    // Update is called once per frame
    void Update()
    {
        //Inform about time
        if (player_turn)
        {
            //playerTime.SetText=(player_turn_time-pTimer).ToString("0.00");
            //machineTime.SetText = (machine_turn_time).ToString("0.00");

            pTimer += Time.deltaTime;
            mTimer = 0;
        }
        else
        {
            //machineTime.SetText = (machine_turn_time - mTimer).ToString("0.00");
            //playerTime.SetText = (player_turn_time).ToString("0.00");

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
            turns_survived++;
            camera.n_turns = turns_survived;
            camera.PushPlayer();
            camera.SetPlayerTime(player_turn_time);
            yield return new WaitForSeconds(player_turn_time);
            AdvanceTurn();
        }
        else
        {
            camera.PushMachine();
            camera.SetMachineTime(machine_turn_time);
            const float killAnimTime = 1;

            yield return new WaitForSeconds(killAnimTime);

            if (IAMovement.Instance.KillPlayer())
            {
                Debug.Log("END GAMEEE");
            }
            else
            {
                yield return new WaitForSeconds(machine_turn_time-killAnimTime);
                if (IAMovement.Instance.showThreats)
                {
                    foreach (ChessPieces mark in IAMovement.Instance.pieces)
                    {
                        mark.UnMarkThreatsTile();
                    }
                }

                IAMovement.Instance.DecideNextMovement();
                AdvanceTurn();

                if (IAMovement.Instance.showThreats)
                {
                    foreach (ChessPieces mark in IAMovement.Instance.pieces)
                    {
                        mark.MarkThreatsTile();
                    }
                }
            }
        }
        
    }
}
