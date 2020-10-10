using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaAnimator : MonoBehaviour
{
    CanvasGroup group;
    public float targetAlpha;

    void Start()
    {
        group = GetComponent<CanvasGroup>();
    }

    void FixedUpdate()
    {
        if (group.alpha < targetAlpha)
            group.alpha += 0.1f;
        else if (group.alpha > targetAlpha)
            group.alpha -= 0.1f;
    }
}
