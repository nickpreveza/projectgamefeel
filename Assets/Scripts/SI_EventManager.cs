using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SI_EventManager : MonoBehaviour
{
    public static SI_EventManager Instance;

    public event Action onCameraMoved;

    void Awake()
    {
        Instance = this;
    }

    public void OnCameraMoved()
    {
        onCameraMoved?.Invoke();
    }
}
