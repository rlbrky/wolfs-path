using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CinematicBars : MonoBehaviour
{
    [SerializeField] Button ShowButton;
    [SerializeField] Button HideButton;

    private RectTransform top_bar, bottom_bar;

    private float changeSizeAmount;
    private float targetSize;
    private bool isActive;

    private void Awake()
    {
        GameObject gameObject = new GameObject("top_Bar", typeof(Image));
        gameObject.transform.SetParent(transform, false);
        gameObject.GetComponent<Image>().color = Color.black;
        top_bar = gameObject.GetComponent <RectTransform>();
        top_bar.anchorMin = new Vector2(0, 1);
        top_bar.anchorMax = new Vector2(1, 1);
        top_bar.sizeDelta = new Vector2(0, 0);

        gameObject = new GameObject("bottom_Bar", typeof(Image));
        gameObject.transform.SetParent(transform, false);
        gameObject.GetComponent<Image>().color = Color.black;
        bottom_bar = gameObject.GetComponent<RectTransform>();
        bottom_bar.anchorMin = new Vector2(0, 0);
        bottom_bar.anchorMax = new Vector2(1, 0);
        bottom_bar.sizeDelta = new Vector2(0, 0);

        //gameObject = new GameObject("ShowButton", typeof(Button));
        //gameObject.transform.SetParent(transform, false);
        //gameObject.GetComponent<RectTransform>().position = new Vector2(0, -200);
        //gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Show";
        ShowButton.onClick.AddListener(() =>  Show(300, .3f) );

        //gameObject = new GameObject("HideButton", typeof(Button));
        //gameObject.transform.SetParent(transform, false);
        //gameObject.GetComponent<RectTransform>().position = new Vector2(0, 200);
        //gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Hide";
        HideButton.onClick.AddListener(() => Hide(.3f));
    }

    private void Update()
    {
        if (isActive)
        {
            Vector2 sizeDelta = top_bar.sizeDelta;
            sizeDelta.y += changeSizeAmount * Time.deltaTime;
            if (changeSizeAmount > 0)
            {
                if (sizeDelta.y >= targetSize)
                {
                    sizeDelta.y = targetSize;
                    isActive = false;
                }
            }
            else
            {
                if (sizeDelta.y <= targetSize)
                {
                    sizeDelta.y = targetSize;
                    isActive = false;
                }
            }
            top_bar.sizeDelta = sizeDelta;
            bottom_bar.sizeDelta = sizeDelta;
        }
    }

    public void Show(float targetSize, float time)
    {
        this.targetSize = targetSize;
        changeSizeAmount = (this.targetSize - top_bar.sizeDelta.y) / time;
        isActive = true;
    }

    public void Hide(float time)
    {
        targetSize = 0;
        changeSizeAmount = (targetSize - top_bar.sizeDelta.y) / time;
        isActive = true;
    }
}
