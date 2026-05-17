using System;
using UniAquarium.Core.Paints;
using UnityEditor;
using UnityEngine.UIElements;

namespace UniAquarium.Core.Components
{
    public abstract class CanvasSceneComponent<T, TActor, TOption> : VisualElement, IDisposable
        where T : CanvasScene<TActor> where TOption : ISceneOption<TActor> where TActor : IActor
    {
        private readonly bool _interactive;
        private readonly PickingMode _originalPickingMode;
        private bool _enabled;
        private T _scene;
        public TOption SceneOption { get; private set; }

        protected CanvasSceneComponent(bool interactive = true)
        {
            _interactive = interactive;
            _originalPickingMode = pickingMode;

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
            if (_enabled) return;
            _enabled = true;

            generateVisualContent -= OnGenerateVisualContent;
            generateVisualContent += OnGenerateVisualContent;

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            if (_interactive)
                RegisterCallback<MouseDownEvent>(OnMouseDown);
            else
                pickingMode = PickingMode.Ignore;
        }

        public virtual void Disable()
        {
            if (!_enabled) return;
            _enabled = false;

            generateVisualContent -= OnGenerateVisualContent;
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            EditorApplication.update -= Update;

            if (_interactive)
                UnregisterCallback<MouseDownEvent>(OnMouseDown);
            else
                pickingMode = _originalPickingMode;
        }

        public virtual void Update()
        {
            EnsureScene();
            _scene?.Update(DeltaTime);
            MarkDirtyRepaint();
        }

        protected abstract void Initialize(TOption sceneOption);
        protected abstract T CreateScene();
        protected abstract TOption CreateSceneOption(T scene);
        protected virtual void ResizeSceneOption(TOption sceneOption, float width, float height)
        {
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            var painter = context.painter2D;

            EnsureScene();
            _scene?.Draw(painter, DeltaTime);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            EnsureScene();
            _scene?.Press(evt);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            EnsureScene(evt.newRect.width, evt.newRect.height);
            if (SceneOption == null) return;

            ResizeSceneOption(SceneOption, evt.newRect.width, evt.newRect.height);
        }

        private void EnsureScene()
        {
            EnsureScene(null, null);
        }

        private void EnsureScene(float? width, float? height)
        {
            if (_scene != null) return;
            if (!HasResolvedSize() && (!width.HasValue || !height.HasValue || !IsValidSize(width.Value) ||
                                      !IsValidSize(height.Value))) return;

            _scene = CreateScene();
            SceneOption = CreateSceneOption(_scene);
            if (width.HasValue && height.HasValue)
                ResizeSceneOption(SceneOption, width.Value, height.Value);
            Initialize(SceneOption);
        }

        private bool HasResolvedSize()
        {
            return IsValidSize(resolvedStyle.width) && IsValidSize(resolvedStyle.height);
        }

        private static bool IsValidSize(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f;
        }
    }
}
