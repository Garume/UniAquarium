# UniAquarium クラス設計レビュー

目的: クラス設計を主眼に、現状の問題点と修正方針を整理する。

## 総評

`CanvasScene -> Actor -> Node -> Shape` という分解は良い方向です。魚・餌・波紋を巨大な継承階層で表現せず、描画や移動や入力反応を Node として合成しているため、挙動の差し替え余地があります。

一方で、抽象層である `Core` が `Aquarium` 固有型を知っていること、`SceneOption` が環境値・サービスロケータ・デバッグ状態を兼ねていること、Actor/Scene/Node のライフサイクル責務が少し混ざっていることが、拡張性と保守性の主な制約になっています。

## 実装進捗

以下はレビュー後のリファクタリングで対応済みです。

- `Core.Paints` から `AquariumActor` への直接依存を削除。
- `Actor.Instantiate()` を削除し、Scene 側の `Spawn()` に生成登録責務を移動。
- `CanvasScene` の内部管理を `List<TActor>` に変更。
- Destroy 済み Actor 削除時に次要素をスキップする問題を修正。
- `ISceneUtility.GetActor<T>()` を廃止し、`GetActors<T>()` に集約。
- Receiver が SpawnerNode を探す構造を廃止し、Scene 内 Actor を直接 query する形に変更。
- `SpawnerNode.Actors` の public mutable list を削除。
- `FishFactory.Create()` の `null` 返却を例外に変更。
- settings 配列 null とデフォルト設定再生成の問題を修正。
- `CanvasSceneComponent.SceneOption` を private setter property に変更。
- `CanvasSceneComponent.Enable()` の二重登録を防止。
- `interactive: false` 時の `pickingMode` 復元を追加。
- `AquariumSceneOption.Resize()` と `GeometryChangedEvent` 経由の viewport 更新を追加。
- 描画コール内の Scene 初期化副作用を `EnsureScene()` に分離し、有効なサイズが取れるまで初期化を遅延。
- Boid 計算の重み付けと 1 要素以下 guard を修正。
- 外部拡張向けに `Node<TOption>` / `RenderNode<TOption>` / `Shape` を public 化。
- Unity Test Runner 用の Editor test assembly と、Scene/Receiver/Settings のテストを追加。
- Unity 同梱参照と .NET Standard 参照を使い、Editor assembly と Test assembly の C# コンパイルを確認。
- update/draw hot path の LINQ、`ToList()`、重複 `Vector2.Distance()`、描画中の短命配列/Stack 生成を削減。
- Unity batchmode 用の performance benchmark entry point と managed-only fallback benchmark を追加。

未対応または次フェーズ候補です。

- `SceneOption` のさらなる分割。
- Node 更新順の phase 化。
- Movement 系 Node が同一 Transform を直接書き換える構造の整理。
- 外部拡張 API としての builder 設計。
- 入力イベントの consume/priority 設計。
- Unity Test Runner の実行確認。現環境では Unity ライセンス未有効のため未実行。ただし Editor/Test assembly のコンパイル確認は完了。

## 優先度の目安

- P0: 実行時バグ、コンパイル/NullReference/破綻しやすい初期化順
- P1: 設計境界を壊し、拡張や保守のコストを大きく上げる問題
- P2: 中期的に効く整理、公開 API、責務分割、テスト容易性
- P3: 命名、細部、将来改善

## 問題点と修正方針

| 優先度 | 領域 | 問題点 | 該当箇所 | 修正方針 |
|---|---|---|---|---|
| P1 | 依存関係 | `Core.Paints` が `AquariumActor` に依存しており、汎用 Core になっていない。 | `Core/Paints/Scene/ISceneOption.cs` | `ISceneOption<TActor>` にする、または `ISceneContext` を Core 側に定義して `AquariumActor` 依存を排除する。 |
| P1 | 依存関係 | `SceneOption.Utility` が `ISceneUtility<AquariumActor>` 固定で、ジェネリック設計と矛盾している。 | `ISceneOption.cs`, `AquariumSceneOption.cs` | `ISceneOption<TActor>`、`ISceneUtility<TActor>`、`CanvasSceneComponent<TScene, TActor, TOption>` の型パラメータを一貫させる。 |
| P1 | 責務分離 | `SceneOption` が画面サイズ、デバッグ状態、TimeScale、Scene 操作 API をまとめて持っている。 | `AquariumSceneOption.cs` | `SceneEnvironment`、`SceneServices`、`DebugOptions` などに分割する。少なくとも読み取り値と操作 API を分ける。 |
| P1 | ライフサイクル | `Actor.Instantiate()` が `SceneOption.Utility` 経由で自身を Scene に登録するため、Actor が所属 Scene の登録方法まで知っている。 | `Actor.cs` | `scene.Spawn(actor, transform)` または `Spawner/Factory` 側に登録責務を寄せる。Actor は自身の状態と Node 更新に集中させる。 |
| P1 | 型安全性 | `CanvasScene<TActor>` が内部で `List<IPaintable>` を持つため、Actor 管理なのか Paintable 管理なのか曖昧。 | `CanvasScene.cs` | `List<TActor>` に寄せる。Actor 以外も描きたいなら `SceneObject` などの共通基底を明示する。 |
| P0 | 更新処理 | Destroy 済み要素を `RemoveAt(index)` した後、次の要素をスキップする可能性がある。 | `CanvasScene.Update()` | 後ろからループする、または削除時に `index--` する。 |
| P0 | 更新処理 | `SpawnerNode.Update()` も `Actors.Remove(actor)` により要素スキップの可能性がある。 | `SpawnerNode.cs` | 後ろからループする、または `RemoveAll(x => x.IsDestroyed)` を使う。 |
| P1 | 検索 API | `ISceneUtility.GetActor<T>()` が最初の 1 件だけ返すため、同種 Actor が複数ある設計と相性が悪い。 | `CanvasScene.cs`, `ISceneUtility.cs` | `IEnumerable<T> GetActors<T>()` を追加する。単一取得が必要な場合は `TryGetActor<T>(out T actor)` で意図を明示する。 |
| P0 | 型キャスト | `GetActor<T>()` の戻り値で `(T)_paintables.Find(...)` を `TActor` として返しており、型関係が読みづらく危険。 | `CanvasScene.cs` | `where T : TActor` を活かし、内部リストも `TActor` にして `return _actors.OfType<T>().FirstOrDefault();` などにする。 |
| P1 | Service Locator | `FoodReceiverNode` や `ShockwaveReceiverNode` が Scene から Spawner Actor を探し、その Node を取り出している。 | `FoodReceiverNode.cs`, `ShockwaveReceiverNode.cs` | `IFoodSource`、`IShockwaveSource`、`ISceneQuery` などの読み取り用サービスを注入する。Node が Scene 構造を知りすぎないようにする。 |
| P1 | 結合度 | Receiver が特定の SpawnerNode 型に依存しているため、生成方法を変えると受信側も壊れる。 | `FoodReceiverNode.cs`, `ShockwaveReceiverNode.cs` | Receiver は `IReadOnlyList<Food>` のような抽象データソースだけを見る。SpawnerNode 実装には依存しない。 |
| P1 | 初期化順 | `Boid.AddTrackingNode()` は `_boidNode` が `CreateNodes()` 済みであることを前提にしている。 | `Boid.cs`, `AquariumComponent.cs` | `Boid` コンストラクタに tracking targets を渡す、または `Initialize()` 後にしか呼べない API として guard を入れる。 |
| P0 | Null 安全性 | `FishFactory.Create()` が default で `null` を返すため、設定値追加時に後続で NullReference になりやすい。 | `FishFactory.cs` | `ArgumentOutOfRangeException` を投げる、または `TryCreate` パターンにする。 |
| P0 | Null 安全性 | `AquariumComponent.Initialize()` が `settings.FishGroupSettings` や `FishSettings` の null を想定していない。 | `AquariumComponent.cs`, `UniAquariumSettings.cs` | 設定読み込み直後に正規化する。`Array.Empty<T>()` を返すプロパティにする。 |
| P0 | Null 安全性 | `AquariumWindow.AddItemsToMenu()` で `SceneOption` が未生成の場合に NullReference になる可能性がある。 | `AquariumWindow.cs` | `SceneOption == null` の間は Debug メニューを disabled にする、または `Enable()` 時に Scene を明示初期化する。 |
| P1 | 初期化タイミング | Scene 作成が `OnGenerateVisualContent` 内にあり、描画が発生するまで Scene/SceneOption が存在しない。 | `CanvasSceneComponent.cs` | `Enable()` または `AttachToPanelEvent` で初期化する。描画メソッドから初期化副作用を外す。 |
| P1 | サイズ管理 | `AquariumSceneOption.Width/Height` が初期化時の `resolvedStyle` 固定で、リサイズに追従しない。 | `AquariumSceneOption.cs`, `CanvasSceneComponent.cs` | `GeometryChangedEvent` でサイズを更新する。`ISceneViewport` のような可変参照を渡す。 |
| P0 | サイズ初期値 | 初回描画時の `resolvedStyle.width/height` が 0 や NaN の場合、ターゲット生成が破綻する可能性がある。 | `AquariumComponent.CreateSceneOption()` | 有効サイズになるまで初期化を遅延する、または fallback サイズを持たせる。 |
| P1 | Enable/Disable | `Enable()` を複数回呼ぶと `RegisterCallback<MouseDownEvent>` が重複する可能性がある。 | `CanvasSceneComponent.cs` | `Disable()` を先に呼ぶ、または `_enabled` フラグで idempotent にする。 |
| P2 | Enable/Disable | `interactive == false` のとき `pickingMode = Ignore` にするが、Disable 時に元に戻さない。 | `CanvasSceneComponent.cs` | 元の `pickingMode` を保持して復元する。 |
| P1 | 公開 API | `AquariumActor` は public だが、`RenderNode` と `Shape` が internal のため、外部から独自魚を作りにくい。 | `AquariumActor.cs`, `RenderNode.cs`, `Shape.cs` | 公開する拡張ポイントを決める。`Shape` と基本 Node を public にするか、`CustomAquariumActor` 用の public builder API を用意する。 |
| P2 | カプセル化 | `CanvasSceneComponent.SceneOption` が public field で、外部から差し替え可能。 | `CanvasSceneComponent.cs` | `public TOption SceneOption { get; private set; }` にする。外部変更が必要な値は明示的なメソッドやオプションにする。 |
| P2 | カプセル化 | `SpawnerNode.Actors` が mutable な `List<T>` として公開されている。 | `SpawnerNode.cs` | `IReadOnlyList<T>` を公開し、内部変更は SpawnerNode に閉じ込める。 |
| P2 | カプセル化 | `TargetTrackingReceivedData` が public field のデータクラスで、不完全な状態を作れる。 | `TargetTrackingNode.cs` | immutable record/class にする。`OnArrived` は nullable を明示し、必須値はコンストラクタで受ける。 |
| P1 | Node 間通信 | `ReceiverNode.ReceivedItem` を `TargetTrackingNode` がポーリングしており、更新順に依存する。 | `ReceiverNode.cs`, `TargetTrackingNode.cs`, `Fish.cs` | `Update` の前後フェーズを分ける、または Receiver が `TryReceive(out data)` でその場計算する。イベント/メッセージキューも候補。 |
| P1 | 更新順 | `Actor.Update()` は Node の追加順に依存する。`TargetTrackingNode` が Receiver より先に更新されると 1 フレーム遅れる。 | `Actor.cs`, `Fish.cs` | Node に priority/order を持たせる。例: Receiver phase -> Behavior phase -> Render state phase。 |
| P2 | Node 初期化 | `AquariumActor.Initialize()` が呼ばれるたびに `CreateNodes()` して追加するため、再初期化で Node が重複する。 | `AquariumActor.cs` | `_initialized` guard を入れる。Node 作成はコンストラクタまたは factory に寄せる。 |
| P2 | Transform 所有 | Node が `ITransform` を保持して Actor の位置を直接変更するため、どの Node が最終的な位置を決めるか曖昧。 | `Node.cs`, `TargetTrackingNode.cs`, `SwayFallingNode.cs` | Movement 系 Node を 1 つに制限する、または複数 Node の出力を合成する `MotionController` を置く。 |
| P2 | 描画責務 | `Shape` がアニメーション状態を持っており、描画と時間更新が混ざっている。 | `FishShape.cs`, `JellyFishShape.cs`, `BranchShape.cs` | 表現としては許容可能。ただしテストや再利用を重視するなら `UpdateVisualState` と `Draw` を分ける。 |
| P2 | 入力設計 | `Press` が全 Actor/Node に broadcast され、イベント消費や優先順位がない。 | `CanvasScene.Press()`, `Actor.Press()` | `bool HandlePress(...)` を返す形にし、消費済みなら後続へ渡さない。入力対象、背景クリック、UI 操作を分ける。 |
| P2 | 入力座標 | `FoodSpawnerNode` は `evt.mousePosition.x` と y=0 で餌を生成し、クリック位置 y は使わない。仕様としてはよいが Press API からは意図が読み取れない。 | `FoodSpawnerNode.cs` | `SpawnFoodAtSurface(x)` のように意図をメソッド名に出す。 |
| P2 | Domain Model | `Food.TargetNode` が `INode` 型で、所有者が誰かを表すには抽象度が低い。 | `Food.cs`, `FoodReceiverNode.cs` | `IFoodTarget` や `TargetReservation` を導入する。餌の予約状態を Food 側に閉じ込める。 |
| P2 | Boid 計算 | `BoidNode.CalculateVectorToCenter()` は tracking node が 1 つ以下だと `_trackingNodes.Count - 1` で割る危険がある。 | `BoidNode.cs` | 2 未満なら boid 計算をスキップする guard を入れる。 |
| P2 | Boid 計算 | `CalculateVectorToAvoid` と `CalculateVectorToAlign` が `result *=` で過去の合算結果ごと倍率をかけるため、意図した重み付けとずれる可能性がある。 | `BoidNode.cs` | 各ベクトルをローカルで重み付けしてから `result += weightedVector` する。 |
| P2 | 速度モデル | `TargetTrackingNode` で `Transform.Velocity = velocity * deltaTime` としており、Velocity が「速度」ではなく「フレーム移動量」になっている。 | `TargetTrackingNode.cs` | `Velocity` は単位時間あたりの速度に統一し、`Position += Velocity * deltaTime` にする。 |
| P2 | Sentinel 値 | `TargetPosition == Vector2.zero` を「ターゲットなし」として扱うため、左上原点をターゲットにできない。 | `TargetTrackingNode.cs` | `Vector2? TargetPosition` または `bool HasTarget` の明示フィールドを使う。 |
| P2 | 角度計算 | `FoodReceiverNode` の角度差が単純な `Abs(selfAngle - angleDiff)` で、-π/π 境界をまたぐと誤判定する。 | `FoodReceiverNode.cs` | `Mathf.DeltaAngle` 相当のラジアン版を使う、または `Vector2.Angle` を使う。 |
| P2 | ランダム | `UniAquariumSettings.Instance.AquariumSetting` が null の場合、getter のたびに `GetDefaultSetting()` が呼ばれランダム設定が変わり得る。 | `UniAquariumSettings.cs` | default を一度生成して `_aquariumSetting` に代入する。 |
| P2 | 設定責務 | `UniAquariumSettings` が設定保持、Asset 作成、デフォルト生成、色変換をすべて持っている。 | `UniAquariumSettings.cs` | `DefaultAquariumSettingsFactory`、`ColorParser`、`SettingsAssetMenu` に分ける。 |
| P2 | 設定 API | `FishSetting` などが mutable property を公開しているため、実行中にどこからでも変更できる。 | `UniAquariumSettings.cs` | Unity serialization との兼ね合いを見つつ、runtime では readonly DTO に変換して使う。 |
| P2 | 命名 | `AquariumSetting` と `UniAquariumSettings` の単数/複数の意味がやや曖昧。 | `UniAquariumSettings.cs` | `AquariumConfig`, `FishConfig`, `FishGroupConfig`, `UniAquariumSettingsAsset` などに整理する。 |
| P2 | Factory | `FishFactory` が enum switch で閉じているため、外部から魚種追加しづらい。 | `FishFactory.cs` | `IFishActorFactory` 登録制、または `FishSetting` に ScriptableObject factory を持たせる。 |
| P2 | Scene 構築 | `AquariumComponent.Initialize()` が設定読み込み、Spawner 作成、魚作成、Boid 接続まで担っている。 | `AquariumComponent.cs` | `AquariumSceneBuilder` を作り、Component は VisualElement と Scene lifecycle のみに集中させる。 |
| P2 | EditorWindow | Reload 処理が Window 内で VisualElement の破棄/再生成を直接行っている。 | `AquariumWindow.cs` | `AquariumComponent.Reload()` を用意し、Window はメニューイベントを委譲する。 |
| P3 | 継承 | `ShockwaveSpawner` と `ShockwaveSpawnerNode` が継承可能な `internal class` だが、拡張意図がなければ sealed でよい。 | `ShockwaveSpawner.cs`, `ShockwaveSpawnerNode.cs` | 継承を想定しない型は `sealed` にする。 |
| P3 | 不要 using | いくつかのファイルに未使用 using がある。 | 複数ファイル | IDE の formatter/analyzer で整理する。 |
| P3 | コメント | `BoidNode` のコメントだけ日本語で、他のコードは英語中心。 | `BoidNode.cs` | OSS として英語に統一するか、プロジェクト方針として日本語に寄せる。 |
| P3 | テスト容易性 | Unity Editor 依存とランダムが直接入っており、ロジック単体テストが難しい。 | 全体 | Boid/Tracking/Receiver の計算部分を Unity Editor 非依存の pure class に切り出す。Random provider を注入する。 |

## 推奨リファクタリング順

1. `CanvasScene` と `SpawnerNode` の削除スキップ問題を修正する。
2. `FishFactory.Create()` の `null` 返却を例外または `TryCreate` に変える。
3. `SceneOption` の null/初期化タイミング問題を直し、Scene 初期化を描画から分離する。
4. `Core` から `AquariumActor` 依存を外す。
5. `CanvasScene` の内部リストを `List<TActor>` にして型安全にする。
6. `ISceneUtility` を `GetActor<T>()` 中心から `GetActors<T>()` / query service 中心へ変える。
7. Receiver が SpawnerNode を直接探す構造を、`IFoodSource` / `IShockwaveSource` などの読み取りインターフェースへ置き換える。
8. `AquariumSceneBuilder` を導入して、`AquariumComponent.Initialize()` からシーン構築責務を外す。
9. 外部拡張用 API を決める。`Shape`/`RenderNode` を公開するか、別の public extension point を作る。
10. Movement/Receiver/Render の Node 更新順を phase として明示する。

## 目指す構造案

```text
UniAquarium.Core
  Scene
    CanvasScene<TActor>
    ISceneContext<TActor>
    ISceneQuery<TActor>
  Actors
    Actor<TContext>
    IActor
    ITransform
  Nodes
    INode
    Node<TContext>
    RenderNode
    MovementNode
  Rendering
    Shape

UniAquarium.Aquarium
  AquariumComponent
  AquariumWindow
  AquariumSceneBuilder
  AquariumSceneContext
  Sources
    IFoodSource
    IShockwaveSource
  Actors
    Fish
    Food
    Shockwave
    Boid
  Nodes
    TargetTrackingNode
    FoodReceiverNode
    ShockwaveReceiverNode
```

## すぐ直すなら最小パッチ候補

### 1. 削除スキップの修正

`CanvasScene.Update()` は削除時にインデックスを戻し、既存の更新順を維持する。

```csharp
for (var index = 0; index < _actors.Count; index++)
{
    var actor = _actors[index];
    actor.Update(deltaTime);

    if (!actor.IsDestroyed) continue;

    _actors.RemoveAt(index);
    index--;
}
```

### 2. Factory の null 返却禁止

```csharp
return fishSetting.FishType switch
{
    FishType.Fish => new Fish(fishSetting.Color, sceneOption),
    FishType.JellyFish => new JellyFish(fishSetting.Color, sceneOption),
    FishType.Lophophorata => new Lophophorata(fishSetting.Color, sceneOption),
    _ => throw new ArgumentOutOfRangeException(nameof(fishSetting.FishType), fishSetting.FishType, null)
};
```

### 3. SceneOption の公開 field を property にする

```csharp
public TOption SceneOption { get; private set; }
```

### 4. `GetActor<T>()` より `GetActors<T>()`

```csharp
IEnumerable<T> GetActors<T>() where T : TActor;
```

Receiver 側は「最初の Spawner を探す」のではなく、必要な対象一覧を query する形に寄せる。現在の実装では `GetActor<T>()` は廃止済み。

## 残す価値がある設計

- Actor が薄く、Node 合成で振る舞いを作っている点。
- `Shape` と `RenderNode` で描画差し替えができる点。
- `AquariumComponent` を `VisualElement` として作っており、EditorWindow 以外にも埋め込める点。
- `FoodReceiverNode` と `ShockwaveReceiverNode` のように、刺激への反応を TargetTracking から分離しようとしている点。

## 結論

このプロジェクトの設計で一番良いところは、早い段階で「魚 = クラス継承」ではなく「Actor に Node を合成する」方向を選んでいることです。直すべき中心は、その合成モデルを支える Core 境界と Scene lifecycle です。

小さく安全に直すなら P0 から始め、大きく設計を磨くなら `Core` の Aquarium 依存排除と `SceneOption` の分割を最初の山にするのがよいです。
