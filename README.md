# UniAquarium

Create your aquarium in Unity Editor.

The aquarium component is provided using Painter2D, which allows HtmlCanvas-like drawing.

![1a3ce99dbad557ae2e9e3f61f785d4b7](https://github.com/Garume/UniAquarium/assets/80187947/77a5f4fa-c0a1-4a40-b9eb-944358e115d3)

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

It is developed with reference to https://github.com/le-nn/vscode-vector-aquarium

## Setup

### Requitements

* Unity 2022.1 or later

### Installation 

1. Open Package Manager from Window > Package Manager.
2. Click the "+" button > Add package from git URL.
3. Enter the following URL:

```
https://github.com/Garume/UniAquarium.git?path=/Assets/UniAquarium
```
### How to open an aquarium

Press `Window > Aquarium` from the toolbar to open the aquarium window.

## Feature

* When tapped, bait appears and fish will chase and eat it.
* When fish tapped, fish will diffuse and escape.
* Other fish will swim.
* Grouped fish swim in swarm.
* You can make your own fish.

## Usage

### Can Feed
Click anywhere on the window and the bait will appear.

The fish will eat this bait.

![6cc0eade3a232596be1dc028a2d34e78](https://github.com/Garume/UniAquarium/assets/80187947/2817c007-d9ca-48fd-9cff-f0a799076205)

### Can Clean

Click anywhere on the window to generate a shockwave.

The fish will run away from this shockwave.

![756f394d0574921db130b7615f0cd055](https://github.com/Garume/UniAquarium/assets/80187947/d9163214-1d7b-4977-88f8-a359bef7a861)

### Custom Settings
You can create Scriptable Objects for custom settings.

UniAquariumSettings from Assets > Create > UniAquarium Settings.

![a8b2fd80d8bc89cb5d27a8791be0b4bd](https://github.com/Garume/UniAquarium/assets/80187947/78590bb9-9088-4067-a2aa-89cae0e65297)

Reload window after making any changes to the settings.

The Aquarium window has a reload function.

`3-point reader > Reload`

![409c10bd298d7723b2fe670bcc0dc503](https://github.com/Garume/UniAquarium/assets/80187947/a40071aa-127d-4c57-86bd-683c702937e4)

### Aquarium Component

Components extending from VisualElement are available to draw aquariums in addition to windows.

By adding this component to any container, you can easily draw an aquarium.

Let's try drawing the aquarium in the Scene Hierarchy Window.

```cs
using System.Reflection;
using UniAquarium.Aquarium;
using UnityEditor;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class HierarchyWindowHook
{
    private static VisualElement _rootVisualElement;

    static HierarchyWindowHook()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (_rootVisualElement != null) return;
        
        // get SceneHierarchyWindow.
        var hierarchyWindowType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var hierarchyWindow = EditorWindow.GetWindow(hierarchyWindowType);

        // get VisualElement using reflection.
        if (hierarchyWindow == null) return;
        var fieldInfo = hierarchyWindowType.GetField("m_SceneHierarchy",
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo == null) return;
        var sceneHierarchy = fieldInfo.GetValue(hierarchyWindow);
        var sceneHierarchyType = sceneHierarchy.GetType();
        var visualElementFieldInfo = sceneHierarchyType.GetField("m_EditorWindow",
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (visualElementFieldInfo == null) return;

        var treeView = visualElementFieldInfo.GetValue(sceneHierarchy);
        var root = treeView as EditorWindow;

        _rootVisualElement = root.rootVisualElement;
        
        // Setting it to false disables clicks and other actions.
        var aquariumComponent = new AquariumComponent(false);
        _rootVisualElement.Add(aquariumComponent);
        aquariumComponent.Enable();
    }
}
```

Write it in any file and save it.
Then, look at the Scene Hierarchy Window.

![974dc08edc6087847922347b928b8745](https://github.com/Garume/UniAquarium/assets/80187947/ed2b4591-a2f3-45a3-8ff5-110d8068307c)

So Nice!

## License
[MIT License](LICENSE)



