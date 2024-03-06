using UnityEngine;

public class UIAdaptation : MonoBehaviour
{
    public RectTransform uiElement; // ��Ҫ����Ӧ��UIԪ��

    private void Start()
    {
        AdaptToScreenHeight();
    }

    private void Update()
    {
        // ÿ֡�����Ļ�߶��Ƿ����仯
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
