using System;
using System.Diagnostics;
using System.IO;
using UniAquarium.Aquarium.Actors;
using UniAquarium.Aquarium.Nodes;
using UniAquarium.Aquarium.Scene;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UniAquarium.Performance
{
    public static class UniAquariumPerformanceBenchmark
    {
        private const int DefaultGroups = 20;
        private const int DefaultFishPerGroup = 8;
        private const int DefaultFoods = 64;
        private const int DefaultShockwaves = 16;
        private const int DefaultFrames = 2000;
        private const float DeltaTime = 1f / 60f;

        public static void RunHeadless()
        {
            var args = Environment.GetCommandLineArgs();
            var outputPath = GetArg(args, "-uniaquariumBenchmarkOutput") ??
                             Path.Combine(Directory.GetCurrentDirectory(), "UniAquariumBenchmark.json");
            var result = Run(
                GetIntArg(args, "-uniaquariumBenchmarkGroups", DefaultGroups),
                GetIntArg(args, "-uniaquariumBenchmarkFishPerGroup", DefaultFishPerGroup),
                GetIntArg(args, "-uniaquariumBenchmarkFoods", DefaultFoods),
                GetIntArg(args, "-uniaquariumBenchmarkShockwaves", DefaultShockwaves),
                GetIntArg(args, "-uniaquariumBenchmarkFrames", DefaultFrames)
            );

            File.WriteAllText(outputPath, JsonUtility.ToJson(result, true));
            Debug.Log($"UniAquarium benchmark written to {outputPath}");
            EditorApplication.Exit(0);
        }

        public static BenchmarkResult Run(int groups, int fishPerGroup, int foods, int shockwaves, int frames)
        {
            var scene = new AquariumScene();
            var option = new AquariumSceneOption(1280f, 720f, scene);

            for (var group = 0; group < groups; group++)
            {
                var boid = new Boid(option);
                scene.Spawn(boid, Vector2.zero, 0f, 1f);

                for (var i = 0; i < fishPerGroup; i++)
                {
                    var fish = new Fish(Color.HSVToRGB((float)group / groups, 0.75f, 0.9f), option);
                    scene.Spawn(fish, new Vector2(i * 8f, group * 8f), 0f, 1f);
                    boid.AddTrackingNode(fish.GetNode<TargetTrackingNode>());
                }
            }

            for (var i = 0; i < foods; i++)
                scene.Spawn(new Food(option), new Vector2(i * 7f % option.Width, 10f), 0f, 1f);

            for (var i = 0; i < shockwaves; i++)
                scene.Spawn(new Shockwave(option, 30f), new Vector2(i * 53f % option.Width, i * 31f % option.Height), 0f, 1f);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var stopwatch = new Stopwatch();
            var beforeMemory = GC.GetTotalMemory(true);
            var beforeAllocatedBytes = GC.GetAllocatedBytesForCurrentThread();
            stopwatch.Start();

            for (var frame = 0; frame < frames; frame++)
                scene.Update(DeltaTime);

            stopwatch.Stop();
            var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - beforeAllocatedBytes;
            var afterMemory = GC.GetTotalMemory(false);

            return new BenchmarkResult
            {
                Groups = groups,
                FishPerGroup = fishPerGroup,
                Foods = foods,
                Shockwaves = shockwaves,
                Frames = frames,
                TotalMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
                MillisecondsPerFrame = stopwatch.Elapsed.TotalMilliseconds / frames,
                AllocatedBytes = allocatedBytes,
                ManagedMemoryDeltaBytes = afterMemory - beforeMemory
            };
        }

        public static BenchmarkResult RunManagedCore(int actors, int nodesPerActor, int frames)
        {
            var scene = new BenchmarkScene();
            var option = new BenchmarkSceneOption(1280f, 720f, scene);

            for (var i = 0; i < actors; i++)
                scene.Spawn(new BenchmarkActor(option, nodesPerActor), new Vector2(i, i), 0f, 1f);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var stopwatch = new Stopwatch();
            var beforeMemory = GC.GetTotalMemory(true);
            var beforeAllocatedBytes = GC.GetAllocatedBytesForCurrentThread();
            stopwatch.Start();

            for (var frame = 0; frame < frames; frame++)
                scene.Update(DeltaTime);

            stopwatch.Stop();
            var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - beforeAllocatedBytes;
            var afterMemory = GC.GetTotalMemory(false);

            return new BenchmarkResult
            {
                Groups = actors,
                FishPerGroup = nodesPerActor,
                Frames = frames,
                TotalMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
                MillisecondsPerFrame = stopwatch.Elapsed.TotalMilliseconds / frames,
                AllocatedBytes = allocatedBytes,
                ManagedMemoryDeltaBytes = afterMemory - beforeMemory
            };
        }

        private static string GetArg(string[] args, string name)
        {
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == name)
                    return args[i + 1];

            return null;
        }

        private static int GetIntArg(string[] args, string name, int fallback)
        {
            var value = GetArg(args, name);
            return int.TryParse(value, out var parsed) ? parsed : fallback;
        }
    }

    [Serializable]
    public sealed class BenchmarkResult
    {
        public int Groups;
        public int FishPerGroup;
        public int Foods;
        public int Shockwaves;
        public int Frames;
        public double TotalMilliseconds;
        public double MillisecondsPerFrame;
        public long AllocatedBytes;
        public long ManagedMemoryDeltaBytes;
    }

    internal sealed class BenchmarkScene : Core.Paints.CanvasScene<BenchmarkActor>
    {
    }

    internal sealed class BenchmarkSceneOption : Core.Paints.ISceneOption<BenchmarkActor>
    {
        public BenchmarkSceneOption(float width, float height, Core.Paints.ISceneUtility<BenchmarkActor> utility)
        {
            Width = width;
            Height = height;
            Utility = utility;
        }

        public float Width { get; }
        public float Height { get; }
        public Core.Paints.ISceneUtility<BenchmarkActor> Utility { get; }
    }

    internal sealed class BenchmarkActor : Core.Paints.Actor<BenchmarkActor, BenchmarkSceneOption>
    {
        private readonly int _nodeCount;

        public BenchmarkActor(BenchmarkSceneOption sceneOption, int nodeCount) : base(sceneOption)
        {
            _nodeCount = nodeCount;
        }

        public override void Initialize()
        {
            for (var i = 0; i < _nodeCount; i++)
                AddNode(new BenchmarkNode());

            base.Initialize();
        }
    }

    internal sealed class BenchmarkNode : Core.Paints.Node<BenchmarkSceneOption>
    {
        public override void Update(float deltaTime)
        {
            var position = Transform.Position;
            position.x += deltaTime;
            position.y += deltaTime * 0.5f;
            Transform.Position = position;
        }
    }
}
