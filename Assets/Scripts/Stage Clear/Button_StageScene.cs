using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_StageScene : MonoBehaviour
{
   

    public void onButtonClick()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
