using System;
using UniAquarium.Core.Paints;
using UnityEditor;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Components
{
    public abstract class CanvasSceneComponent<T, TActor, TOption> : VisualElement, IDisposable
        where T : CanvasScene<TActor> where TOption : ISceneOption where TActor : IActor
    {
        private readonly bool _interactive;
        private T _scene;
        protected TOption SceneOption;

        protected CanvasSceneComponent(bool interactive = true)
        {
            _interactive = interactive;

            style.height = Length.Percent(100f);
            style.width = Length.Percent(100f);
        }

        protected abstract float DeltaTime { get; }

        public void Dispose()
        {
            Disable();
        }

        public virtual void Enable()
        {
            generateVisualContent -= OnGenerateVisualContent;
            generateVisualContent += OnGenerateVisualContent;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            if (_interactive)
                RegisterCallback<MouseDownEvent>(OnMouseDown);
            else
                pickingMode = PickingMode.Ignore;
        }

        public virtual void Disable()
        {
            generateVisualContent -= OnGenerateVisualContent;
            EditorApplication.update -= Update;

            if (_interactive)
                UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public virtual void Update()
        {
            _scene?.Update(DeltaTime);
            MarkDirtyRepaint();
        }

        protected abstract void Initialize(TOption sceneOption);
        protected abstract T CreateScene();
        protected abstract TOption CreateSceneOption(T scene);

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            var painter = context.painter2D;

            if (_scene == null)
            {
                _scene = CreateScene();
                SceneOption = CreateSceneOption(_scene);
                Initialize(SceneOption);
            }

            _scene?.Draw(painter, DeltaTime);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            _scene?.Press(evt);
        }
    }
}