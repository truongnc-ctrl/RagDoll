using System;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
public class player : MonoBehaviour
{
    public InputAction actions;
    void Awake()
    {
        actions = new InputAction();
    }
    
}
