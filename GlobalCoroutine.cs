using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalCoroutine
{
    public delegate Coroutine StartCoroutine(IEnumerator routine);
    public delegate Coroutine StartCoroutineMethod(string methodName, object value);
    public delegate Coroutine StartCoroutineAuto(IEnumerator routine);
    public delegate void StopAllCoroutines();
    public delegate void StopCoroutine(IEnumerator routine);
    public delegate void StopCoroutineMethod(string methodName);

    public static StartCoroutine Start;
    public static StartCoroutineMethod StartMethod;
    public static StartCoroutineAuto StartAuto;
    public static StopCoroutine Stop;
    public static StopCoroutineMethod StopMethod;
    public static StopAllCoroutines StopAll;
}
