using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieces : MonoBehaviour
{
    public int _x, _y;
    public virtual Tile[] GetPosibleMovements() {
        return new Tile[0];
    }

    public virtual void Start()
    {
        MoveTo(_x,_y);
    }

    public virtual void MoveTo(int x, int y)
    {
        _x = x; _y = y;
        transform.position = Tile.Position(x, y);
    }

    public Tile CheckExistTile(int x, int y)
    {
        RaycastHit hit;

        if (Physics.Raycast(Tile.Position(x, y) + new Vector3(0, 10, 0), -Vector3.up, out hit, 100.0f))
        {
            Debug.Log("Found an object - distance: " + hit.distance);
            return hit.transform.gameObject.GetComponent<Tile>();
        }
        else
        {
            return null;
        }
    }

}
