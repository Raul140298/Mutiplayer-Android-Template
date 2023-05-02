using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lean.Touch;

public enum eDirection
{
    None,
    Up,
    Right,
    Down,
    Left
}

public class InputController : MonoBehaviour
{
    public event Action<eDirection> onSwipe;

    void OnEnable()
    {
        LeanTouch.OnFingerSwipe += OnSwipeDetected;
    }

    void OnDisable()
    {
        LeanTouch.OnFingerSwipe -= OnSwipeDetected;
    }

    private void OnSwipeDetected(LeanFinger finger)
    {
        onSwipe?.Invoke(CalculateSwipeDirection(finger.SwipeScaledDelta));
    }

    private eDirection CalculateSwipeDirection(Vector2 swipeDelta)
    {
        float deltaX = swipeDelta.x;
        float deltaY = swipeDelta.y;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) // HORIZONTAL SWIPE
        {
            return (deltaX > 0) ? eDirection.Right : eDirection.Left;
        }
        else // VERTICAL SWIPE
        {
            return (deltaY > 0) ? eDirection.Up : eDirection.Down;
        }
    }
}
