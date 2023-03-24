using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FaderController : MonoBehaviour
{
    public static FaderController instance;
    [SerializeField] private Image faderImage;

    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        FadeClose(1f);
    }

    public void FadeClose(float time = 1f, float delay = 0f)
    {
        faderImage.DOFade(0f, time).SetDelay(delay).OnComplete(() => { faderImage.raycastTarget = false; });
    }

    public void FadeOpen(float time)
    {
        faderImage.raycastTarget = true;
        faderImage.DOFade(1f, time);
    }
}

