using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unigine;
using UnigineApp;
using Console = Unigine.Console;

[Component(PropertyGuid = "358f6eb0ea01b24f35477b6278cb4359ef47d4e4")]
public class DebugLib
{
    public DebugLib()
    {
        Visualizer.Enabled = true;
        drawDatas = new DrawData[0];
    }

    private DrawData[] drawDatas;

    private struct DrawData
    {
        public int ID { get; }
        public vec3 Start { get; }
        public vec3 End { get; }
        public vec4 Color { get; }
        public float Time { get; set; }

        public string Type { get; }
        
        public float Radius { get; }
        
        public vec3 Dir { get; }
        
        public float ArrowSize { get; }
        
        public int ScreenSpace { get; }

        // Line constructor
        public DrawData(vec3 start, vec3 end, vec4 color, float time)
        {
            Start = start;
            End = end;
            Color = color;
            Time = time;
            ID = 0;
            Type = "Line";
            Radius = 0.0F;
            Dir = vec3.ZERO;
            ArrowSize = 0.0F;
            ScreenSpace = 0;
        }

        // Sphere constructor.
        public DrawData(vec3 start, vec4 color, float time, float radius)
        {
            ID = 0;
            Start = start;
            End = new vec3();
            Color = color;
            Time = time;
            Type = "Sphere";
            Radius = radius;
            Dir = vec3.ZERO;
            ArrowSize = 0.0F;
            ScreenSpace = 0;
        }

        public DrawData(vec3 start, vec3 dir, vec4 color, float time, float arrowSize, int screenSpace)
        {
            ID = 0;
            Start = start;
            End = vec3.ZERO;
            Color = color;
            Time = time;
            Type = "Direction";
            Radius = 0.0F;
            Dir = dir;
            ArrowSize = 0.25F;
            ScreenSpace = 1;
        }
    }

    private const float ClearTime = 1.0F;
    private float currentClearTime = 0.0F;

    public void Update()
    {
        UpdateDrawData();
        ClearDebug();
    }

    private void ClearDebug()
    {
        if (currentClearTime <= 0.0F)
        {
            currentClearTime = ClearTime;
            ProcessClearDebug();
        }
        else
        {
            currentClearTime -= App.GetIFps();
        }
    }

    private static void ProcessClearDebug()
    {
        // var iFps = App.GetIFps();
    }

    private void UpdateDrawData()
    {
        for(var i = 0; i < drawDatas.Length; i++)
        {
            var drawData = drawDatas[i];
            if (drawData.Time <= 0.0F) continue;
            switch (drawData.Type)
            {
                case "Line":
                    Visualizer.RenderLine3D(drawData.Start, drawData.End, drawData.Color);
                    break;
                case "Sphere":
                    var transform = new mat4();
                    transform.Translate = drawData.Start;
                    Visualizer.RenderSphere(drawData.Radius, transform, drawData.Color);
                    break;
                case "Direction":
                    Visualizer.RenderDirection(
                        drawData.Start, drawData.Dir, drawData.Color, 
                        drawData.ArrowSize, drawData.ScreenSpace);
                    break;
                default:
                    Log.Message($"Unknown draw type: {drawData.Type}\n");
                    break;
            }

            drawDatas[i].Time -= App.GetIFps();
        }
    }

    public void DrawLine(vec3 start, vec3 end, vec4 color, float time)
    {
        drawDatas = Add(drawDatas, new DrawData(start, end, color, time));
    }

    public void DrawSphere(vec3 position, vec4 color, float time, float radius)
    {
        drawDatas = Add(drawDatas, new DrawData(position, color, time, radius));
    }

    public void DrawDirection(vec3 start, vec3 dir, vec4 color, float time, float arrowSize = 0.25F, int screenSpace = 1)
    {
        drawDatas = Add(drawDatas, new DrawData(start, dir, color, time, arrowSize, screenSpace));
    }

    private static T[] Add<T>(T[] target, params T[] items)
    {
        // Validate the parameters
        if (target == null) {
            target = new T[] { };
        }
        if (items== null) {
            items = new T[] { };
        }

        // Join the arrays
        var result = new T[target.Length + items.Length];
        target.CopyTo(result, 0);
        items.CopyTo(result, target.Length);
        return result;
    }
}