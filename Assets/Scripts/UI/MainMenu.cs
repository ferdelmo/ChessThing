using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    public GameObject main;
    public GameObject Difficult;
    public GameObject Threats;

    bool showThreats = true;

    public EventSystem es;

    public GameObject yes;
    public GameObject easy;

    bool firstT = true;
    bool firstD = true;

    int difficult = 0; //0 easy, 1 med, 2 hard

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        main.SetActive(false);
        Threats.SetActive(true); 
        if (firstT)
        {
            es.SetSelectedGameObject(yes);
            firstT = false;
        }
    }

    public void OnThreatsSubmited()
    {
        Threats.SetActive(false);
        Difficult.SetActive(true);
        if (firstD)
        {
            es.SetSelectedGameObject(easy);
            firstD = false;
        }
    }

    public void Yes()
    {
        showThreats = true;
        OnThreatsSubmited();
    }

    public void No()
    {
        showThreats = false;
        OnThreatsSubmited();
    }

    public void Easy()
    {
        difficult = 0;
        StartLevel();
    }

    public void Medium()
    {
        difficult = 1;
        StartLevel();
    }

    public void Hard()
    {
        difficult = 2;
        StartLevel();
    }

    public void StartLevel() {
        Debug.Log("PLAY WITH " + showThreats + " " + difficult);
        IAMovement.Instance.Reset();
        IAMovement.Instance.showThreats = showThreats;
        IAMovement.Instance.difficult = difficult;
        SceneManager.LoadScene("Level");
    }


    public void Exit()
    {
        Application.Quit();
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
