using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CategoryHolder : MonoBehaviour
{
    public enum TextCategory
    {
        A,
        B,
        C
    }
    public TextCategory category;

    public enum Textofnumber
    {
        one,
        two,
        three
    }
    public Textofnumber textofnumber;

    public enum Textofline
    {
        one,
        two,
        three
    }
    public Textofline textofline;
}
