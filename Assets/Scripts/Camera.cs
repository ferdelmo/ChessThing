using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Player follow;

    public int x_offset, y_offset, h_offset;

    public GameObject clock;

    public Transform player_clock, machine_clock;

    public Transform player_push, machine_push;

    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = -transform.position + clock.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        transform.position = follow.transform.position + new Vector3(x_offset, h_offset, y_offset);
        transform.position = new Vector3(transform.position.x, h_offset, transform.position.z);

        clock.transform.position = new Vector3(transform.position.x + offset.x,clock.transform.position.y, clock.transform.position.z);
    }

    public void PushPlayer()
    {
        StartCoroutine(Push(player_push, 0.10f));
        StartCoroutine(Push(player_push, -0.10f));
    }

    public void PushMachine()
    {
        StartCoroutine(Push(player_push, -0.10f));
        StartCoroutine(Push(player_push, 0.10f));
    }

    IEnumerator Push(Transform t, float h)
    {
        const float AnimDur = 0.4f;

        float startH = transform.position.y;
        float counter = 0;

        while (counter < AnimDur)
        {
            counter += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, startH + h * (counter / AnimDur), transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x,  h, transform.position.z);
    }
}
