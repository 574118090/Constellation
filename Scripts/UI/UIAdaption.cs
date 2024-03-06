using UnityEngine;

public class UIAdaptation : MonoBehaviour
{
    public RectTransform uiElement; // 需要自适应的UI元素

    private void Start()
    {
        AdaptToScreenHeight();
    }

    private void Update()
    {
        // 每帧检测屏幕高度是否发生变化
        if (Screen.height != uiElement.sizeDelta.y)
        {
            AdaptToScreenHeight();
        }
    }

    private void AdaptToScreenHeight()
    {
        float screenHeight = Screen.height / 0.3f;
        float screenWidth = Screen.width * 0.3f / 0.3f;
        uiElement.sizeDelta = new Vector2(screenWidth, screenHeight);
    }
}
