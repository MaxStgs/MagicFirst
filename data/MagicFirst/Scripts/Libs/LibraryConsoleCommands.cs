using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unigine;
using UnigineApp;
using Console = Unigine.Console;

[Component(PropertyGuid = "581e8cd6a2ff5e3d76e80a78c01b4b773c82326a")]
public class LibraryConsoleCommands
{
    // Init all commands.
    public LibraryConsoleCommands()
    {
        Unigine.Console.AddCommand(
            "get_object_forward",
            "Command for ray casting before camera and Log object",
            GetObjectForward);
        Interpreter.AddExternFunction("get_object_forward2", SimpleObjectForward, "");

        Console.AddCommand("jump_sp",
            "Jump to spawn point, by default jump to last, or call `jump_sp 2` it will second jump point",
            JumpToSpawnPoint);
    }

    private static void SimpleObjectForward()
    {
        Log.Message("SimpleObjectForward\n");
    }


    private static void GetObjectForward(int argc, string[] argv)
    {
        // var length = 100;
        var intersection = AdditionLib.GetObjectBeforePlayer();

        Log.Message(intersection != null
            ? $"GetObjectForward(): {intersection.Name}\n"
            : "GetObjectForward(): Object is not found\n");
    }

    private static void JumpToSpawnPoint(int argc, string[] argv)
    {
        var nodes = new List<Node>();
        World.GetNodes(nodes);

        var indexForJump = -1;
        if (argv.Length > 1)
        {
            int.TryParse(argv[1], out indexForJump);
        }
        else
        {
            indexForJump = AppWorldLogic.CurrentWorld.LastSpawnPointIndex;
        }

        if (indexForJump == -1)
        {
            Log.Error("JumpToSpawnPoint indexForJump got -1\n");
        }

        foreach (var node in nodes)
        {
            var nodeRef = node as NodeReference;
            if (nodeRef == null)
            {
                continue;
            }

            var child = nodeRef.FindChild("SpawnPoint");
            if (child == -1)
            {
                continue;
            }

            var spawnPoint = nodeRef.GetChild(child);
            var spawnPointComponent = spawnPoint.GetComponent<SpawnPointComponent>();

            if (spawnPointComponent == null)
            {
                Log.Message($"For spawn point with id: {spawnPoint.ID} not found SpawnPointComponent\n");
                continue;
            }

            if (spawnPointComponent.Index != indexForJump) continue;

            Log.Message("Found\n");

            var cuboid = World.GetNodeByName("Cuboid");
            if (cuboid == null)
            {
                Log.Message("Cuboid is null!\n");
                return;
            }

            cuboid.Position = spawnPointComponent.GetSpawnPosition();
            var fpsComponent = Game.Player.GetComponent<FirstPersonController>();
            if (fpsComponent != null)
            {
                fpsComponent?.Hack_UpdatePosition(spawnPointComponent.GetSpawnPosition());
            }
            else
            {
                Game.Player.Transform = spawnPointComponent.GetSpawnTransform();
            }

            return;
        }

        Log.Message($"JumpToSpawnPoint can not find SpawnPoint with index: {indexForJump}\n");
    }
}