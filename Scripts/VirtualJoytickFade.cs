using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Terresquall;

public class VirtualJoystickFade : MonoBehaviour
{
    VirtualJoystick joystick;
    public float activeTime = 2f;
    public float fadeTime = 1f; // Instead of fadeTime, this sets speed of fading
    public float fadeAlpha = 0.2f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Image joystickImage;
    private Image controlStickImage;

    private Coroutine fadeCoroutine;

    void Start()
    {
        joystick = GetComponent<VirtualJoystick>();
        joystickImage = GetComponent<Image>();
        controlStickImage = joystick.controlStick.GetComponent<Image>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && joystick.currentPointerId == -2)
        {
            SetAlpha(1f);
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        }

        if (Input.GetMouseButtonUp(0) && joystick.currentPointerId == -1)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(StartDissapearance());
        }
    }

    IEnumerator StartDissapearance()
    {
        // Wait while active
        float time = 0f;
        while (time < activeTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // Get current alpha from joystick image
        float startAlpha = joystickImage ? joystickImage.color.a : 1f;

        // Fade out using fade speed and easing
        time = 0f;
        float duration = Mathf.Abs(startAlpha - fadeAlpha) / fadeTime;

        while (time < duration)
        {
            float t = time / duration;
            float easedT = fadeCurve.Evaluate(t);
            float alpha = Mathf.Lerp(startAlpha, fadeAlpha, easedT);
            SetAlpha(alpha);

            time += Time.deltaTime;
            yield return null;
        }

        // Final fade and disable
        SetAlpha(fadeAlpha);
    }

    void SetAlpha(float alpha)
    {
        if (joystickImage)
        {
            Color c = joystickImage.color;
            c.a = alpha;
            joystickImage.color = c;
        }

        if (controlStickImage)
        {
            Color c = controlStickImage.color;
            c.a = alpha;
            controlStickImage.color = c;
        }
    }
}
