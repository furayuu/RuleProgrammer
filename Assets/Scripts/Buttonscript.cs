using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttonscript : MonoBehaviour
{
    public enum Mode
    {
        Drag,
        Copy
    }

    public static Mode currentmode = Mode.Drag;

    public void SetDragMode()
    {
        currentmode = Mode.Drag;
    }

    public  void SetCopyMode()
    {
        currentmode = Mode.Copy;
    }
}
