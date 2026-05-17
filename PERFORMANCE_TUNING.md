# UniAquarium Performance Tuning

## Goal

Update/draw hot paths were tuned to reduce per-frame allocations and repeated expensive operations. A headless benchmark entry point was added for Unity batchmode measurement.

## Optimizations Applied

- Removed LINQ/`ToList()` from receiver hot paths.
- Replaced repeated `Vector2.Distance()` calls with squared-distance checks where possible.
- Added `ISceneUtility<TActor>.Actors` so receivers can scan the scene without allocating typed enumerables.
- Replaced the allocating `GetActors<T>()` iterator with a caller-owned `List<T>` buffer API.
- Added typed `AquariumScene` indexes for `Food` and `Shockwave`, so receivers no longer scan every actor.
- Replaced per-detection `TargetTrackingReceivedData` class and arrival `Action` allocations with a value-type payload.
- Replaced `CreateNodes()` with allocation-free `ActorBuilder` composition so actor node setup is explicit without array creation.
- Kept `Painter2DScope` as a reference type to preserve mutable transform semantics.
- Reduced Boid update work by precomputing group position/velocity totals once per update.
- Replaced additional `Vector2.normalized` hot-path calls with explicit guarded normalization.
- Reworked `JellyFishShape` head geometry so fill/frame use the same edge point calculation and added cached cap sine/cosine/power values.
- Replaced `Vector2.zero` target sentinel logic in `TargetTrackingNode` with explicit target state, so `(0, 0)` is a valid target.
- Removed unused `INode.Id` / `Guid.NewGuid()` identity management from nodes.
- Added scene spawn guardrails for null actors and duplicate actor spawn attempts.
- Added scene spawn guardrails for destroyed actor reuse.
- `GetActors<T>(List<T>)` now owns clearing the caller-provided buffer before writing results.
- Forced target arrival now clears `TargetTrackingNode` target state immediately.
- Tightened `AquariumSceneOption` construction to require an `AquariumScene` instead of accepting a generic utility and casting internally.
- Added `AllocatedBytes` measurement using `GC.GetAllocatedBytesForCurrentThread()`.
- Replaced `foreach` in hot update/draw loops with indexed `for` loops.
- Removed per-frame fin-side array allocation from `FishShape`.
- Removed per-frame `Stack<Vector2>` allocation from `JellyFishShape`.
- Removed LINQ array construction from `BranchShape` initialization.
- Avoided duplicate arrival-distance checks in `TargetTrackingNode`.
- Added a headless benchmark entry point: `UniAquarium.Performance.UniAquariumPerformanceBenchmark.RunHeadless`.

## Headless Benchmark Command

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.1f1\Editor\Unity.exe' `
  -batchmode `
  -nographics `
  -projectPath 'C:\Users\nabej\Documents\Github\UniAquarium' `
  -executeMethod UniAquarium.Performance.UniAquariumPerformanceBenchmark.RunHeadless `
  -uniaquariumBenchmarkOutput 'C:\Users\nabej\Documents\Github\UniAquarium\UniAquariumBenchmark.json' `
  -quit `
  -logFile 'C:\Users\nabej\Documents\Github\UniAquarium\UniAquariumBenchmark.log'
```

## Measurement Result

After Unity Hub authentication, the headless benchmark executed successfully.

Observed result on this machine:

```json
{
  "Groups": 20,
  "FishPerGroup": 8,
  "Foods": 64,
  "Shockwaves": 16,
  "Frames": 2000,
  "TotalMilliseconds": 365.0854,
  "MillisecondsPerFrame": 0.1825427,
  "AllocatedBytes": 0,
  "ManagedMemoryDeltaBytes": 98058240
}
```

`AllocatedBytes` is the primary zero-allocation signal for the measured update section. `ManagedMemoryDeltaBytes` is kept as a coarse process-memory reference and can include runtime/editor memory that has not been collected.

## Test Runner Verification

Unity EditMode tests were executed through batchmode:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.1f1\Editor\Unity.exe' `
  -batchmode `
  -nographics `
  -projectPath 'C:\Users\nabej\Documents\Github\UniAquarium' `
  -runTests `
  -testPlatform editmode `
  -testResults 'C:\Users\nabej\Documents\Github\UniAquarium\UniAquariumTestResults.xml' `
  -logFile 'C:\Users\nabej\Documents\Github\UniAquarium\UniAquariumTest.log'
```

Result:

- 19 tests passed after adding JellyFish geometry, zero-vector target, forced-target cleanup, spawn guardrail, actor-buffer, ActorBuilder, and scene-option constructor coverage.
- 0 failed.
- Performance benchmark tests assert `AllocatedBytes == 0`.
- `JellyFishShapeTests` asserts generated head edge points are symmetric and non-NaN.

## Benchmark Output Format

When Unity licensing is available, `UniAquariumBenchmark.json` will contain:

```json
{
  "Groups": 20,
  "FishPerGroup": 8,
  "Foods": 64,
  "Shockwaves": 16,
  "Frames": 2000,
  "TotalMilliseconds": 0.0,
  "MillisecondsPerFrame": 0.0,
  "AllocatedBytes": 0,
  "ManagedMemoryDeltaBytes": 0
}
```

The default benchmark stresses update-side logic, including fish groups, food actors, shockwaves, target tracking, receiver scans, and boid updates.
