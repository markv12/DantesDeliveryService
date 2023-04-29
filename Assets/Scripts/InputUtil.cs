using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputUtil : MonoBehaviour {
    public static float GetHorizontal() {
        float result = 0;
        Keyboard kb = Keyboard.current;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) {
            result -= 1;
        }
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) {
            result += 1;
        }
        return result;
    }

    public static float GetVertical() {
        float result = 0;
        Keyboard kb = Keyboard.current;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) {
            result -= 1;
        }
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) {
            result += 1;
        }
        return result;
    }

    public static bool GetKeyDown(Key key) {
        return Keyboard.current[key].wasPressedThisFrame;
    }

    public static bool GetKey(Key key) {
        return Keyboard.current[key].isPressed;
    }

    public static float MouseScrollDelta => Mouse.current.scroll.ReadValue().y;

    public static Vector2 MousePosition {
        get {
            if(Pen.current != null && Pen.current.inRange.ReadValue() > 0.5f) {
                return Pen.current.position.ReadValue();
            } else {
                return Mouse.current.position.ReadValue();
            }
        }
    }

    public static bool LeftMouseButtonDown => buttonDownFrame == Time.frameCount;
    public static bool LeftMouseButtonUp => buttonUpFrame == Time.frameCount;
    public static bool LeftMouseButtonIsPressed {
        get {
            return Mouse.current.leftButton.isPressed;
        }
    }

    private static int buttonDownFrame;
    private static int buttonUpFrame;
    private static bool prevIsPressed;

    private void Update() {
        bool isPressed = LeftMouseButtonIsPressed;
        if (isPressed && !prevIsPressed) {
            buttonDownFrame = Time.frameCount;
        } else if (!isPressed && prevIsPressed) {
            buttonUpFrame = Time.frameCount;
        }
        prevIsPressed = isPressed;
    }

    private IEnumerator ResetRoutine() {
        yield return null;
        yield return null;
        yield return null;
        InputSystem.ResetDevice(Mouse.current, true);
    }

    public static bool SelectPressed() {
        return GetKeyDown(Key.E) || GetKeyDown(Key.Space);
    }
    public static bool CTRLPressed => GetKey(Key.LeftCtrl) || GetKey(Key.RightCtrl) || GetKey(Key.LeftCommand) || GetKey(Key.RightCommand);
    public static bool ShiftPressed => GetKey(Key.LeftShift) || GetKey(Key.RightShift);
}
