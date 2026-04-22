using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextClipboard : MonoBehaviour
{
    //コピーしたものを記憶
    public static string copiedText = "";
    public static GameObject copiedObject = null;
    public static CategoryHolder.TextCategory copiedCategory;
    public static CategoryHolder.Textofnumber copiednumber;
}
