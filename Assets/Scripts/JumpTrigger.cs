using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    public enum JumpDirection
    {
        Right,
        Left
    }

    [Header("Jump Direction")]
    public JumpDirection jumpDirection;
}