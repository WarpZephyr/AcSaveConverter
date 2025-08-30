using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;

namespace AcSaveConverter.Input
{
    public static class InputTracker
    {
        private static readonly HashSet<Key> CurrentlyPressedKeys = [];
        private static readonly HashSet<Key> NewKeysThisFrame = [];

        private static readonly HashSet<MouseButton> CurrentlyPressedMouseButtons = [];
        private static readonly HashSet<MouseButton> NewMouseButtonsThisFrame = [];

        private static readonly Queue<string> DragDropFileQueue = [];

        private static Vector2 MousePosition;
        private static Vector2 MouseDelta;

        public static Vector2 GetMousePosition()
            => MousePosition;

        public static Vector2 GetMouseDelta()
            => MouseDelta;

        public static bool GetKey(Key key)
        {
            return CurrentlyPressedKeys.Contains(key);
        }

        public static bool GetKeyDown(Key key)
        {
            return NewKeysThisFrame.Contains(key);
        }

        public static bool GetMouseButton(MouseButton button)
        {
            return CurrentlyPressedMouseButtons.Contains(button);
        }

        public static bool GetMouseButtonDown(MouseButton button)
        {
            return NewMouseButtonsThisFrame.Contains(button);
        }

        public static void UpdateFrameInput(InputSnapshot snapshot, Sdl2Window window)
        {
            NewKeysThisFrame.Clear();
            NewMouseButtonsThisFrame.Clear();

            MousePosition = snapshot.MousePosition;
            MouseDelta = window.MouseDelta;
            for (int i = 0; i < snapshot.KeyEvents.Count; i++)
            {
                KeyEvent ke = snapshot.KeyEvents[i];
                if (ke.Down)
                {
                    KeyDown(ke.Key);
                }
                else
                {
                    KeyUp(ke.Key);
                }
            }

            for (int i = 0; i < snapshot.MouseEvents.Count; i++)
            {
                MouseEvent me = snapshot.MouseEvents[i];
                if (me.Down)
                {
                    MouseDown(me.MouseButton);
                }
                else
                {
                    MouseUp(me.MouseButton);
                }
            }
        }

        public static void HandleDragDrop(DragDropEvent e)
        {
            DragDropFileQueue.Enqueue(e.File);
        }

        public static bool HasDragDrop()
        {
            return DragDropFileQueue.Count > 0;
        }

        public static string GetDragDrop()
        {
            return DragDropFileQueue.Dequeue();
        }

        private static void MouseUp(MouseButton mouseButton)
        {
            CurrentlyPressedMouseButtons.Remove(mouseButton);
            NewMouseButtonsThisFrame.Remove(mouseButton);
        }

        private static void MouseDown(MouseButton mouseButton)
        {
            if (CurrentlyPressedMouseButtons.Add(mouseButton))
            {
                NewMouseButtonsThisFrame.Add(mouseButton);
            }
        }

        private static void KeyUp(Key key)
        {
            CurrentlyPressedKeys.Remove(key);
            NewKeysThisFrame.Remove(key);
        }

        private static void KeyDown(Key key)
        {
            if (CurrentlyPressedKeys.Add(key))
            {
                NewKeysThisFrame.Add(key);
            }
        }
    }
}
