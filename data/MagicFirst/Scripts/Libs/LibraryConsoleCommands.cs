using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unigine;
using UnigineApp;

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
        Unigine.Interpreter.AddExternFunction("get_object_forward2", SimpleObjectForward, "");
        Unigine.Console.AddCommand("ShowAllMethodsOfLib", "show all methods", ShowAllMethodsOfLib);
    }

    private void SimpleObjectForward()
    {
        Log.Message("SimpleObjectForward\n");
    }
    

    private void GetObjectForward(int argc, string[] argv)
    {
        // var length = 100;
        var intersection = AdditionLib.GetObjectBeforePlayer();
        
        Log.Message(intersection != null
            ? $"GetObjectForward(): {intersection.Name}\n"
            : "GetObjectForward(): Object is not found\n");
    }

    private static void ShowAllMethodsOfLib(int argc, string[] argv)
    {
        var a = Assembly.LoadFile("D:/Projects/Games/Examples/Unigine/Poligone_project/bin/Poligone_project_x64.dll");
        var types = a.GetTypes();
        foreach (var type in types)
        {
            if (!type.IsPublic)
            {
                continue;
            }

            var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var member in members)
            {
                System.Console.WriteLine(type.Name+"."+member.Name);
            }
        }
    }
}