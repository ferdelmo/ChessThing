using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int start_x, start_y;

    int _x, _y;

    TileGenerator tg;
    TurnManager tm;

    public GameObject pawn;

    public int x {
        get { return _x; }
    }

    public int y {
        get { return _y; }
    }


    public void SetPosition(int x, int y)
    {
        _x = x; _y = y;

        //transform.position = Tile.Position(x, y);
        StartCoroutine(MoveToAnim(x,y));
    }

    public void Start()
    {
        SetPosition(start_x, start_y);

        tg = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TileGenerator>();
        tm = GameObject.FindGameObjectWithTag("TileGenerator").GetComponent<TurnManager>();
    }

    static float AnimDur = .3f;
    public IEnumerator MoveToAnim(int x, int y)
    {

        Vector3 startPos = transform.position;

        float counter = 0;

        while (counter < AnimDur)
        {
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, Tile.Position(x,y), counter / AnimDur);
            transform.position = new Vector3(transform.position.x, 1 * ChessPieces.Arch(counter / AnimDur), transform.position.z);
            yield return null;
        }
        transform.position = Tile.Position(x, y);
    }

    //Unable the box colliders and eneble the mesh collider and add a rigidbody for the kill
    public void PrepareToKill()
    {
        GetComponent<BoxCollider>().enabled = false;
        foreach (BoxCollider bc in GetComponentsInChildren<BoxCollider>())
        {
            bc.enabled = false;
        }

        pawn.GetComponent<MeshCollider>().enabled = true;

        pawn.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tm.IsPlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SetPosition(_x + 1, _y);
                tg.NewLine();
                tm.AdvanceTurn();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                SetPosition(_x + 2, _y);
                tg.NewLine();
                tg.NewLine();
                tm.AdvanceTurn();
            }
        }
    }
}
