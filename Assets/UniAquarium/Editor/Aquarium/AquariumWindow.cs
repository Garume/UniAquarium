using UnityEditor;
using UnityEngine;

namespace UniAquarium.Aquarium
{
    public sealed class AquariumWindow : EditorWindow, IHasCustomMenu
    {
        private AquariumComponent _aquariumComponent;

        public void OnEnable()
        {
            _aquariumComponent = new AquariumComponent();
            rootVisualElement.Add(_aquariumComponent);

            _aquariumComponent.Enable();
        }

        public void OnDisable()
        {
            _aquariumComponent.Dispose();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Debug"), _aquariumComponent.SceneOption.IsDebug,
                () => { _aquariumComponent.SceneOption.IsDebug = !_aquariumComponent.SceneOption.IsDebug; });
            menu.AddItem(new GUIContent("Reload"), false, () =>
            {
                rootVisualElement.Remove(_aquariumComponent);
                _aquariumComponent.Dispose();
                _aquariumComponent = new AquariumComponent();
                rootVisualElement.Add(_aquariumComponent);
                _aquariumComponent.Enable();
            });
        }

        [MenuItem("Window/Aquarium")]
        public static void OpenWindow()
        {
            Open();
        }

        private static void Open()
        {
            var window = GetWindow<AquariumWindow>();
            window.titleContent = new GUIContent("Aquarium");
            window.Show();
        }
    }
}