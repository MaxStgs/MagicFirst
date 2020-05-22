using Unigine;
using Object = Unigine.Object;

[Component(PropertyGuid = "cee1a0d9592b94649e89547340490f36187a3d1d")]
public static class AdditionLib
{
	public static vec3 GetForwardVector(Node node)
	{
		// Camera.

		// If it Camera, handle as NZ-forward
		if (node.IsPlayer)
		{
			return -((Player) node).Camera.IModelview.AxisZ.Normalize3();
		}

		return node.GetWorldDirection(MathLib.AXIS.Y);
	}

	public static vec3 GetRightVector(Node node)
	{
		var shape = (ShapeCapsule) node.ObjectBody.GetShape(0);
		return node.GetWorldDirection(MathLib.AXIS.X);
	}
	
	public static vec3 GetUpVector(Node node)
	{
		// Camera.
		var playerActor = node as PlayerActor;
		
		// If it Camera, handle as Y-up
		if (playerActor != null)
		{
			return playerActor.GetWorldDirection(MathLib.AXIS.Y);
		}
		// If it is simple object, handle as Z-up.

		return node.GetWorldDirection(MathLib.AXIS.Z);
	}

	public static vec3 GetForwardVectorFromPoint(vec3 start, vec3 direction, float distance)
	{
		return start + direction * distance;
	}

	// Return World.GetIntersection() result.
	public static Object GetObjectBeforePlayer(float length = 100.0F)
	{
		var start = Game.Player.Camera.Position;
		var end = GetForwardVectorFromPoint(
			start, GetForwardVector(Game.Player), length);
        
		return World.GetIntersection(start, end, int.MaxValue);
	}

	public static vec3 RandomDir()
	{
		var x = Game.GetRandomFloat(0, 1);
		var y = Game.GetRandomFloat(0, 1);
		var z = Game.GetRandomFloat(0, 1);
		return new vec3(x, y, z);
	}
	
	public static vec3 RandomDirXY()
	{
		var x = Game.GetRandomFloat(0, 1);
		var y = Game.GetRandomFloat(0, 1);
		return new vec3(x, y, 0.0F);
	}
}