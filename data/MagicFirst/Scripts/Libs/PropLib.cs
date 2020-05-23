using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unigine;

[Component(PropertyGuid = "57c75929d415d0740decc143260075c6d9b7d26d")]
public class PropLib
{
    public static Variable LoadValue(Node node, string propertyName, string name)
    {
        var propOwner = node.Parent == null ? node : node.Parent;
        var propIndex = propOwner.FindProperty(propertyName);
        if (propIndex == -1)
        {
            Log.Error($"PropLib.LoadValue({node.ID}, {propertyName}, {name}) can not find propertyName");
            return null;
        }

        var prop = propOwner.GetProperty(propIndex);
        var ptr = prop.GetParameterPtr(name);

        if (ptr != null) return ptr.GetValue();

        Log.Error($"PropLib.LoadValue({node.ID}, {propertyName}, {name}) can not find name");
        return null;
    }

    // May be make it at C++ and working with any Prop?
    // Not for use
    public static void LoadVariables(Node node, string propertyName, IEnumerable<string> listOfVariables)
    {
        var propOwner = node.Parent == null ? node : node.Parent;
        var propIndex = propOwner.FindProperty(propertyName);
        if (propIndex == -1)
        {
            Log.Error($"PropLib.LoadVariables({node.ID}, {propertyName}) can not find propertyName");
            return;
        }

        var prop = propOwner.GetProperty(propIndex);
        foreach (var variable in listOfVariables)
        {
            // Solve it
            var field = node.GetComponent<BonfireComponent>().GetType().GetProperty(variable);
            if (field == null)
            {
                Log.Error(
                    $"PropLib.LoadVariables({node.ID}, {propertyName}), waiting field with name {variable}, but not found");
                continue;
            }

            var ptr = prop.GetParameterPtr(variable);
            if (ptr == null)
            {
                Log.Error(
                    $"PropLib.LoadVariables({node.ID}, {propertyName}), waiting prop with name {variable}, but not found");
                continue;
            }

            field.SetValue(node, ptr.ValueString);
        }
    }
}