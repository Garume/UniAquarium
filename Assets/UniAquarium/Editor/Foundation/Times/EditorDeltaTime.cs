// Reference: https://github.com/breadnone/EditorDeltaTime

using UnityEditor;
using UnityEngine;

namespace UniAquarium.Foundation
{
    internal static class EditorDeltaTime
    {
        private static float _lastTime;
        private static float _screenRate;

        internal static float DeltaTime { get; private set; }

        internal static long FrameCount { get; private set; }

        internal static bool Paused { get; private set; }

        public static void Start()
        {
            Stop();
            GetScreenRate();
            _lastTime = 0;
            FrameCount = 0;
            GetScreenRate();
            DeltaTime = 0;
            Paused = false;

            EditorApplication.update += EditModeRunner;
        }

        public static void Stop()
        {
            EditorApplication.update -= EditModeRunner;
        }

        public static void Pause(bool state)
        {
            Paused = state;
        }

        private static void GetScreenRate()
        {
            var refValue = Screen.currentResolution.refreshRateRatio.value;
            _screenRate = 1f / (float)refValue;
        }

        private static void EditModeRunner()
        {
            if (Paused || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling ||
                EditorApplication.isUpdating || EditorApplication.isPlaying) return;

            var time = (float)EditorApplication.timeSinceStartup;

            if (time < _lastTime + _screenRate) return;

            var min = time - _lastTime;

            min = Mathf.Min(0.1f, min);

            DeltaTime = min;
            _lastTime = time;
            FrameCount++;

            if (FrameCount == long.MaxValue - 1) FrameCount = 1;
        }
    }
}