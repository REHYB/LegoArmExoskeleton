using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleEvents : MonoBehaviour
{
    // The singleton instance
    public static ExampleEvents current;

    // Singleton operation
    private void Awake()
    {
        current = this;
    }

    // The event to which we will chain the function which is sending the UDP message
    public event Action onMKeyHit;

    public void MKeyHit()
    {
        onMKeyHit?.Invoke();
    }
}
