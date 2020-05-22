using System.Diagnostics;
using Unigine;

[Component(PropertyGuid = "88bd95684ebef9a7b4e92ada82a2fc717653cb1c")]
public class SpawnPointComponent : Component
{
    private WorldTrigger worldTrigger;

    private Node debugSphere;

    private void Init()
    {
        // write here code to be called on component initialization
        worldTrigger = node.GetChild(0) as WorldTrigger;
        debugSphere = node.GetChild(1);
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
        Log.Message($"Enter: {node1.Name}\n");
        if (debugSphere == null) return;
        var mesh = (debugSphere as ObjectMeshStatic);
        if (mesh == null)
        {
            return;
        }

        mesh.SetMaterialParameterFloat4("albedo_color", ColorLib.Blue, 0);
        Engine.RunWorldFunction(new Variable("TrackerWrapper::set"), new Variable(2.5f));
    }
}