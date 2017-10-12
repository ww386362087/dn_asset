using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class XAutoFade
{
    private enum FadeType
    {
        ToBlack,
        ToClear
    }

    private static float _in = 0;
    private static bool _force_from_black = false;

    private static IEnumerator _fadeToBlack = null;
    private static IEnumerator _fadeToClear = null;

    private static Color transparent = new Color(1, 1, 1, 0);
    private static Color black = Color.white;

    private static Image fade { get { return UIManager.singleton.FadeImage; } }

    public static void Update()
    {
        if (_fadeToBlack != null)
        {
            if (!_fadeToBlack.MoveNext())
            {
                _fadeToBlack = null;
            }
        }
        else if (_fadeToClear != null)
        {
            if (!_fadeToClear.MoveNext())
            {
                _fadeToClear = null;
            }
        }
    }
    
    public static void MakeBlack()
    {
        MakeBlack(true);
    }

    public static void MakeBlack(bool stopall)
    {
        if (stopall) StopAll();
        fade.color = black;
    }

    public static void FadeOut2In(float In, float Out)
    {
        _in = In;
        FadeOut(Out);
    }

    public static void FadeIn(float duration, bool fromBlack = false)
    {
        StopAll();
        _force_from_black = fromBlack;

        Start(FadeType.ToClear, duration);
    }

    public static void FadeOut(float duration)
    {
        StopAll();
        Start(FadeType.ToBlack, duration);
    }

    public static void FastFadeIn()
    {
        StopAll();
        fade.color = transparent;
    }

    private static IEnumerator FadeToBlack(float duration)
    {
        float alpha = fade.color.a;
        float rate = 1 / duration;
        float progress = alpha;
        while (progress < 1.0f && fade.color.a < 1.0f)
        {
            fade.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, progress));
            progress += rate * (Time.timeScale != 0f ? Time.deltaTime / Time.timeScale : Time.unscaledDeltaTime);
            yield return null;
        }
        fade.color = black;
        if (_in > 0)
        {
            FadeIn(_in);
            _in = 0;
        }
    }

    private static IEnumerator FadeToClear(float duration)
    {
        float alpha = fade.color.a;
        if (_force_from_black)
        {
            alpha = 1;
            fade.color = black;
        }
        if (duration == 0)
        {
            alpha = 0;
        }
        else
        {
            float rate = 1 / duration;
            float progress = 1 - alpha;
            while (progress < 1.0f && fade.color.a > 0)
            {
                fade.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, progress));
                progress += rate * (Time.timeScale != 0f ? Time.deltaTime / Time.timeScale : Time.unscaledDeltaTime);
                yield return null;
            }
        }
        fade.color = transparent;
    }

    private static void Start(FadeType type, float duration)
    {
        switch (type)
        {
            case FadeType.ToBlack:
                if (_fadeToBlack == null)
                    _fadeToBlack = FadeToBlack(duration);
                break;
            case FadeType.ToClear:
                if (_fadeToClear == null)
                    _fadeToClear = FadeToClear(duration);
                break;
        }
    }

    private static void StopAll()
    {
        _fadeToBlack = null;
        _fadeToClear = null;
    }


    public static void Debug()
    {
        XDebug.Log("bef field in value: ", _in);
    }

    public static void Debug2()
    {
        XDebug.LogGreen("aft field in value:", _in);
    }
}
