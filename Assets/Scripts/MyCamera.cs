using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MyCamera : MonoBehaviour
{
    public Player follow;

    public int x_offset, y_offset, h_offset;

    public GameObject clock;

    public GameObject player_clock, machine_clock;

    public Transform player_push, machine_push;

    Vector3 offset, offset_p, offset_m;

    public float startRot_m = 0;
    public float startRot_p = 270;

    bool first = true;

    public GameObject GameOverCanvas;

    Coroutine rotate = null;

    public Text survived;
    public int n_turns = 0;

    // Start is called before the first frame update
    void Start()
    {
        offset = -transform.position + clock.transform.position;
        offset_p = -transform.position + player_clock.transform.position;
        offset_m = -transform.position + machine_clock.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        transform.position = follow.transform.position + new Vector3(x_offset, h_offset, y_offset);
        transform.position = new Vector3(transform.position.x, h_offset, transform.position.z);

        clock.transform.position = new Vector3(transform.position.x + offset.x, clock.transform.position.y, clock.transform.position.z);
        player_clock.transform.position = new Vector3(transform.position.x + offset_p.x, player_clock.transform.position.y, player_clock.transform.position.z);
        machine_clock.transform.position = new Vector3(transform.position.x + offset_m.x, machine_clock.transform.position.y, machine_clock.transform.position.z);

    }

    public void PushPlayer()
    {
        StartCoroutine(Push(player_push, -0.20f));
        StartCoroutine(Push(machine_push, 0.20f));
    }

    public void PushMachine()
    {
        if (first)
        {
            StartCoroutine(Push(player_push, 0.10f));
            StartCoroutine(Push(machine_push, -0.10f));
            first = false;
        }
        else
        {
            StartCoroutine(Push(player_push, 0.20f));
            StartCoroutine(Push(machine_push, -0.20f));
        }
    }

    public void SetPlayerTime(float t)
    {
        if (rotate != null)
        {
            StopCoroutine(rotate);
        }
        player_clock.transform.rotation = Quaternion.Euler(0-270+ startRot_p, -90, -90);
        rotate = StartCoroutine(RotateClock(player_clock.transform, t, -startRot_p));
    }

    public void SetMachineTime(float t)
    {
        if (rotate!=null)
        {
            StopCoroutine(rotate);
        }
        machine_clock.transform.rotation = Quaternion.Euler(0 - 270 + startRot_m, -90, -90);
        rotate = StartCoroutine(RotateClock(machine_clock.transform, t, -startRot_m));
    }


    IEnumerator RotateClock(Transform t, float time, float rotation)
    {
        float counter = 0;

        while (counter < time)
        {
            counter += Time.deltaTime;
            t.Rotate(0,rotation / time * Time.deltaTime,0);
            yield return null;
        }

        t.rotation = Quaternion.Euler(-270,-90,-90);
    }

    IEnumerator Push(Transform t, float h)
    {
        const float AnimDur = 0.2f;

        float startH = t.position.y;
        float counter = 0;

        while (counter < AnimDur)
        {
            counter += Time.deltaTime;
            t.position = new Vector3(t.position.x, startH + h * (counter / AnimDur), t.position.z);
            yield return null;
        }

        t.position = new Vector3(t.position.x, startH + h, t.position.z);
    }


    public void Exit()
    {
        Debug.Log("Exit Game");
    }

    public void Restart()
    {
        IAMovement.Instance.Reset();
        SceneManager.LoadScene("Level");
    }

    public void ShowGameOver()
    {
        GameOverCanvas.SetActive(true);
        survived.text = "You survived " + n_turns + " turns.";
    }
}
