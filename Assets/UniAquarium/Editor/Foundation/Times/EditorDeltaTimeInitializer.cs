// Reference: https://github.com/breadnone/EditorDeltaTime

using UnityEditor;

namespace UniAquarium.Foundation
{
    [InitializeOnLoad]
    internal static class EditorDeltaTimeInitializer
    {
        static EditorDeltaTimeInitializer()
        {
            EditorApplication.playModeStateChanged += PlayModeState;
            EditorDeltaTime.Start();
        }

        private static void PlayModeState(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.ExitingEditMode:
                    EditorDeltaTime.Stop();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                case PlayModeStateChange.EnteredEditMode:
                    EditorDeltaTime.Start();
                    break;
                default:
                    EditorDeltaTime.Start();
                    break;
            }
        }
    }
}