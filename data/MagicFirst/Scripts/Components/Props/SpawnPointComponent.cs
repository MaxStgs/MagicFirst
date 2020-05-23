using System;
using System.Diagnostics;
using Unigine;
using UnigineApp;

[Component(PropertyGuid = "88bd95684ebef9a7b4e92ada82a2fc717653cb1c")]
public class SpawnPointComponent : Component
{
    private WorldTrigger worldTrigger;

    private Node debugSphere;

    [ShowInEditor] public int Index { get; private set; } = 0;

    private void Init()
    {
        // write here code to be called on component initialization
        worldTrigger = node.GetChild(0) as WorldTrigger;
        debugSphere = node.GetChild(1);
        var result = PropLib.LoadValue(node, "SpawnPointProp", "Index");
        if (result == null)
        {
            Log.Message($"SpawnPointComponent can not find value from SpawnPointProp for Index");
            return;
        }

        Index = result.Int;
        if (worldTrigger == null)
        {
            Log.Message($"Node: {node.Name} can not find PhysicalTrigger inside SpawnPointComponent\n");
        }
        else
        {
            worldTrigger.AddEnterCallback(EnterCallback);
        }
    }

    private void Update()
    {
        // write here code to be called before updating each render frame
        
    }

    private void EnterCallback(Node node1)
    {
        // Log.Message($"Enter: {node1.Name}\n");
        if (debugSphere == null) return;
        var mesh = (debugSphere as ObjectMeshStatic);
        if (mesh == null)
        {
            return;
        }

        mesh.SetMaterialParameterFloat4("albedo_color", ColorLib.Blue, 0);
        AppWorldLogic.CurrentWorld.LastSpawnPointIndex = Index;
        Log.Message($"Current index: {Index}\n");
    }

    public vec3 GetSpawnPosition()
    {
        return worldTrigger.WorldTransform.Translate;
    }

    public mat4 GetSpawnTransform()
    {
        return worldTrigger.GetIWorldTransform();
    }
}