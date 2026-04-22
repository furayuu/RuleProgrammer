using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CopyText : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI text;
    private Color originalColor;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Buttonscript.currentmode != Buttonscript.Mode.Copy)
            return;

        if (eventData.dragging) return;

        // =========================
        // 右クリック → コピー
        // =========================
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 前のハイライト解除
            if (TextClipboard.copiedObject != null)
            {
                var old = TextClipboard.copiedObject.GetComponent<CopyText>();
                if (old != null)
                    old.ResetColor();
            }

            // コピー
            TextClipboard.copiedText = text.text;
            TextClipboard.copiedObject = gameObject;

            var category = GetComponent<CategoryHolder>();
            if (category != null)
            {
                TextClipboard.copiedCategory = category.category;
                TextClipboard.copiednumber = category.textofnumber;
            }

            // ハイライト
            text.color = Color.yellow;

            Debug.Log("コピー: " + text.text);
        }

        // =========================
        // 左クリック → 貼り付け
        // =========================
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // クリップボード空なら何もしない
            if (string.IsNullOrEmpty(TextClipboard.copiedText))
                return;

            var category = GetComponent<CategoryHolder>();
            if (category == null) return;

            if (category.category != TextClipboard.copiedCategory&& category.textofnumber != TextClipboard.copiednumber)
                return;

            // 貼り付け
            text.text = TextClipboard.copiedText;
            Debug.Log("貼り付け: " + TextClipboard.copiedText);

            // ハイライト解除
            if (TextClipboard.copiedObject != null)
            {
                var old = TextClipboard.copiedObject.GetComponent<CopyText>();
                if (old != null)
                    old.ResetColor();
            }

            // クリップボードを空にする
            TextClipboard.copiedText = "";
            TextClipboard.copiedObject = null;
        }
    }

    public void ResetColor()
    {
        text.color = originalColor;
    }
}
