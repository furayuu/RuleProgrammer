using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_Start : MonoBehaviour

{
    [SerializeField] public Canvas Title;

    [SerializeField] public Canvas Select;
    void Start()
    {
        Title.gameObject.SetActive(true);

        Select.gameObject.SetActive(false);
    }
    public void StartButton()
    {
        Title.gameObject.SetActive(false);

        Select.gameObject.SetActive(true);

    }    
}
