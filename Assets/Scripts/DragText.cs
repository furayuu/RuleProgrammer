using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragText : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private static DragText draggingObject;

    private Vector2 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Buttonscript.currentmode != Buttonscript.Mode.Drag)
            return;

        // ドラッグするものと元の位置を記録
        draggingObject = this;
        startPosition = rectTransform.anchoredPosition;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Buttonscript.currentmode != Buttonscript.Mode.Drag)
            return;

        // 移動処理
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingObject == null) return;

        canvasGroup.blocksRaycasts = true;

        // ドロップ先を取得
        GameObject targetObj = eventData.pointerCurrentRaycast.gameObject;

        if (targetObj != null)
        {
            CategoryHolder Me = GetComponent<CategoryHolder>();
            CategoryHolder target = targetObj.GetComponentInParent<CategoryHolder>();
            DragText targetDrag = targetObj.GetComponentInParent<DragText>();

            if (target != null && targetDrag != null && target != Me && target.category != Me.category && target.textofnumber == Me.textofnumber && target.textofline == Me.textofline) 
            {
                // 入れ替え
                Vector2 temp = targetDrag.rectTransform.anchoredPosition;
                targetDrag.rectTransform.anchoredPosition = startPosition;
                rectTransform.anchoredPosition = temp;
                return;
            }
        }

        // ドロップ失敗 → 元に戻す
        rectTransform.anchoredPosition = startPosition;
    }
}
