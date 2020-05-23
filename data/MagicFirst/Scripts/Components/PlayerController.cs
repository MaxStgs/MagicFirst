using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c1b405274d6fc05a7a8993f1b8e67c9ca1056f4b")]
public class PlayerController : Component
{
	//CharacterController
	[ShowInEditor][Parameter(Group = "Control", Tooltip = "Forward axis")]
	private Input.KEY ForwardInputKey = Input.KEY.W;

	[ShowInEditor][Parameter(Group = "Control", Tooltip = "Backward axis")]
	private Input.KEY BackwardInputKey = Input.KEY.S;

	[ShowInEditor][Parameter(Group = "Control", Tooltip = "Left axis")]
	private Input.KEY LeftInputKey = Input.KEY.A;

	[ShowInEditor][Parameter(Group = "Control", Tooltip = "Right axis")]
	private Input.KEY RightInputKey = Input.KEY.D;

	[ShowInEditor][Parameter(Group = "Control", Tooltip = "Jump button")]
	private Input.KEY JumpInputKey = Input.KEY.SPACE;

	[ShowInEditor][Parameter(Group = "Control", Tooltip = "Run button")]
	private Input.KEY SprintInputKey = Input.KEY.SHIFT;

	[ShowInEditor][Parameter(Group = "Control", Tooltip = "Look in direction of motion")]
	bool lookAtDirection = true;

	[ShowInEditor][Parameter(Group = "Active camera", Tooltip = "Current active camera")]
	private PlayerDummy playerDummy = null;

	[ShowInEditor][ParameterMask(Group = "Physical parameters")]
	private int physicalMask = 1;

	[ShowInEditor][ParameterMask(Group = "Physical parameters")]
	private int intersectionMask = 1;

	[ShowInEditor][ParameterMask(Group = "Physical parameters")]
	private int collisionMask = 1;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float physicalMass = 30.0f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float radius = 0.5f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float height = 1.2f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float minFriction = 0.1f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float maxFriction = 1.0f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float minVelocity = 3.0f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float maxVelocity = 6.0f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float acceleration = 8.0f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float damping = 8.0f;

	[ShowInEditor][Parameter(Group = "Physical parameters")]
	private float jumping = 1f;

	public const float PLAYER_ACTOR_IFPS = 1.0f / 60.0f;
	public const float PLAYER_ACTOR_CLAMP = 89.9f;
	public const int PLAYER_ACTOR_COLLISIONS = 4;

	private float PhysicalMass
	{
		get { return shape.Mass; }
		set { shape.Mass = value; }
	}

	private int PhysicalMask
	{
		get { return rigid.PhysicalMask; }
		set { rigid.PhysicalMask = value; }
	}

	private int PhysicsIntersectionMask
	{
		get { return shape.PhysicsIntersectionMask; }
		set { shape.PhysicsIntersectionMask = value; }
	}

	private int CollisionMask
	{
		get { return shape.CollisionMask; }
		set { shape.CollisionMask = value; }
	}

	private float MinFriction
	{
		get { return minFriction; }
		set { minFriction = MathLib.Max(value, 0.0f); }
	}

	private float MaxFriction
	{
		get { return maxFriction; }
		set { maxFriction = MathLib.Max(value, 0.0f); }
	}

	private float MinVelocity
	{
		get { return minVelocity; }
		set { minVelocity = MathLib.Max(value, 0.0f); }
	}

	private float MaxVelocity
	{
		get { return maxVelocity; }
		set { maxVelocity = MathLib.Max(value, 0.0f); }
	}

	private float Acceleration
	{
		get { return acceleration; }
		set { acceleration = MathLib.Max(value, 0.0f); }
	}

	private float Damping
	{
		get { return damping; }
		set { damping = MathLib.Max(value, 0.0f); }
	}

	private float Jumping
	{
		get { return jumping; }
		set { jumping = MathLib.Max(value, 0.0f); }
	}

	public float MaxStepHeight { get; set; }

	private Unigine.Camera camera;
	private ObjectDummy objectDummy;
	private BodyRigid rigid;
	private ShapeCapsule shape;

	private vec3 velocity;
	private vec3 position;
	private vec3 direction;
	private float phiAngle;

	private enum Side { Left, Right }
	private const float bulletOffset = 0.6f;

	enum State
	{
		Forward = 0,
		Backward,
		Left,
		Right,
		Jump,
		Sprint,
		NumStates
	};

	enum StateStatus
	{
		Diasbled = 0,
		Enabled,
		Begin,
		End
	};

	private int[] states = new int[(int)State.NumStates];
	private float[] times = new float[(int)State.NumStates];

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// MAIN METHODS
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	void Init()
	{
		camera = playerDummy.Camera;

		objectDummy = new ObjectDummy();

		rigid = new BodyRigid();
		rigid.MaxAngularVelocity = 0.0f;
		rigid.Freezable = true;

		shape = new ShapeCapsule(radius, height);
		shape.Restitution = 0.0f;
		shape.Continuous = false;

		rigid.Enabled = true;
		objectDummy.Body = rigid;
		shape.Body = rigid;

		position = vec3.ZERO;
		direction = new vec3(1.0f, 0.0f, 0.0f);
		phiAngle = 90.0f;

		for (int i = 0; i < (int)State.NumStates; i++)
		{
			states[i] = 0;
			times[i] = 0.0f;
		}

		UpdateTransform();

		PhysicalMask = physicalMask;
		PhysicsIntersectionMask = intersectionMask;
		CollisionMask = collisionMask;
		PhysicalMass = physicalMass;
		SetCollisionRadius(radius);
		SetCollisionHeight(height);
		MinFriction = minFriction;
		MaxFriction = maxFriction;
		MinVelocity = minVelocity;
		MaxVelocity = maxVelocity;
		Acceleration = acceleration;
		Damping = damping;
		Jumping = jumping;

		Ground = 0;
		Ceiling = 0;

		Game.Player = playerDummy;
	}

	public void Update()
	{
		var ifps = Game.IFps;

		UpdateRigid(ifps);
	}
	

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PUBLIC METHODS
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// transform
	// (note: use these methods if you want to change position of the player.
	// node.WorldTransform doesn't work)
	public void SetTransform(mat4 transform)
	{
		node.Transform = transform;
		UpdateTransform();
	}

	public void SetWorldTransform(mat4 transform)
	{
		node.WorldTransform = transform;
		UpdateTransform();
	}

	private void SetCollisionRadius(float radius)
	{
		if (MathLib.Compare(shape.Radius, radius) < 1)
		{
			var up = vec3.UP;

			rigid.SetPreserveTransform(new mat4(MathLib.Translate(up * (radius - shape.Radius))) * rigid.Transform);
			shape.Radius = radius;
		}
	}

	public float GetCollisionRadius()
	{
		return shape.Radius;
	}

	private void SetCollisionHeight(float height)
	{
		if (MathLib.Compare(shape.Height, height) < 1)
		{
			var up = vec3.UP;

			rigid.SetPreserveTransform(new mat4(MathLib.Translate(up * (height - shape.Height) * 0.5f)) * rigid.Transform);
			shape.Height = height;
		}
	}

	public float GetCollisionHeight()
	{
		return shape.Height;
	}

	// phi angle
	public void SetPhiAngle(float angle)
	{
		angle = angle - phiAngle;
		direction = new quat(vec3.UP, angle) * direction;
		phiAngle += angle;

		FlushTransform();
	}

	public float GetPhiAngle()
	{
		return phiAngle;
	}

	// view direction
	public void SetViewDirection(vec3 view)
	{
		direction = MathLib.Normalize(view);
		vec3 tangent, binormal;
		Geometry.OrthoBasis(vec3.UP, out tangent, out binormal);
		phiAngle = MathLib.Atan2(MathLib.Dot(direction, tangent), MathLib.Dot(direction, binormal)) * MathLib.RAD2DEG;

		FlushTransform();
	}
	
	private int Ground { get; set; }
	private int Ceiling { get; set; }
	
	private mat4 GetBodyTransform()
	{
		vec3 up = vec3.UP;
		vec3 center = position;
		return MathLib.SetTo(center, center + new vec3(direction - up * MathLib.Dot(direction, up)), up) * new mat4(MathLib.RotateX(-90.0f) * MathLib.RotateZ(90.0f));
	}

	private mat4 GetModelview()
	{
		var up = vec3.UP;
		var eye = position + new vec3(up * (shape.Height + shape.Radius));
		return MathLib.LookAt(eye, eye + new vec3(direction), up);
	}

	private int UpdateState(bool condition, int state, int begin, int end, float ifps)
	{
		if (condition && states[state] == (int)StateStatus.Diasbled && begin == 1)
		{
			states[state] = (int)StateStatus.Begin;
			times[state] = 0.0f;
			return (int)StateStatus.Begin;
		}

		if (!condition && (states[state] == (int)StateStatus.Enabled || states[state] == (int)StateStatus.Begin) && end == 1)
		{
			states[state] = (int)StateStatus.End;
			return (int)StateStatus.End;
		}

		if ((condition && states[state] == (int)StateStatus.Begin) || states[state] == (int)StateStatus.Enabled)
		{
			states[state] = (int)StateStatus.Enabled;
			times[state] += ifps;
			return (int)StateStatus.Enabled;
		}

		if (states[state] == (int)StateStatus.End)
		{
			states[state] = (int)StateStatus.Diasbled;
			return (int)StateStatus.Diasbled;
		}

		return (int)StateStatus.Diasbled;
	}

	private void UpdateStates(int enabled, float ifps)
	{

		if (enabled == 1 && !Unigine.Console.Activity)
		{
			if (Input.IsKeyPressed(ForwardInputKey) && Input.IsKeyPressed(BackwardInputKey))
			{
				UpdateState(false, (int)State.Forward, 1, 1, ifps);
				UpdateState(false, (int)State.Backward, 1, 1, ifps);
			}
			else
			{
				UpdateState(Input.IsKeyPressed(ForwardInputKey), (int)State.Forward, 1, 1, ifps);
				UpdateState(Input.IsKeyPressed(BackwardInputKey), (int)State.Backward, 1, 1, ifps);
			}

			if (Input.IsKeyPressed(LeftInputKey) && Input.IsKeyPressed(RightInputKey))
			{
				UpdateState(false, (int)State.Left, 1, 1, ifps);
				UpdateState(false, (int)State.Right, 1, 1, ifps);
			}
			else
			{
				UpdateState(Input.IsKeyPressed(LeftInputKey), (int)State.Left, 1, 1, ifps);
				UpdateState(Input.IsKeyPressed(RightInputKey), (int)State.Right, 1, 1, ifps);
			}

			UpdateState(Input.IsKeyPressed(JumpInputKey), (int)State.Jump, Ground, 1, ifps);
			UpdateState(Input.IsKeyPressed(SprintInputKey), (int)State.Sprint, 1, 1, ifps);
		}

		// disable states
		else
		{
			UpdateState(false, (int)State.Forward, 1, 1, ifps);
			UpdateState(false, (int)State.Backward, 1, 1, ifps);
			UpdateState(false, (int)State.Left, 1, 1, ifps);
			UpdateState(false, (int)State.Right, 1, 1, ifps);
			UpdateState(false, (int)State.Jump, Ground, Ground, ifps);
			UpdateState(false, (int)State.Sprint, 1, 1, ifps);
		}
	}
	
	private void UpdateRigid(float ifps)
	{
		var up = vec3.UP;
		var impulse = vec3.ZERO;
		
		vec3 tangent, binormal;
		Geometry.OrthoBasis(up, out tangent, out binormal);
		
		var x = new quat(up, -phiAngle) * binormal;
		var y = MathLib.Normalize(MathLib.Cross(up, x));
		var z = MathLib.Normalize(MathLib.Cross(x, y));
		
		UpdateStates(1, ifps);
		
		var xVelocity = MathLib.Dot(x, rigid.LinearVelocity);
		var yVelocity = MathLib.Dot(y, rigid.LinearVelocity);
		var zVelocity = MathLib.Dot(z, rigid.LinearVelocity);
		
		phiAngle = -playerDummy.GetWorldRotation().GetAngle(up) - 90;
		
		x = new quat(up, -phiAngle) * binormal;
		y = MathLib.Normalize(MathLib.Cross(up, x));
		z = MathLib.Normalize(MathLib.Cross(x, y));

		if (states[(int)State.Forward] > 0)
			impulse += x;
		if (states[(int)State.Backward] > 0)
			impulse -= x;
		if (states[(int)State.Left] > 0)
			impulse += y;
		if (states[(int)State.Right] > 0)
			impulse -= y;
		impulse.Normalize();

		if (impulse.Length2 > MathLib.EPSILON)
			rigid.Frozen = false;

		if (impulse.Length2 > 0 && lookAtDirection)
		{
			var currentRot = new quat(MathLib.LookAt(vec3.ZERO, direction, vec3.UP, MathLib.AXIS.Y));
			var targetRot = new quat(MathLib.LookAt(vec3.ZERO, impulse, vec3.UP, MathLib.AXIS.Y));

			var rot = MathLib.Slerp(currentRot, targetRot, 7.5f * ifps);
			var delta = rot * MathLib.Inverse(currentRot);

			direction = direction * delta;
			direction.Normalize();
		}
		
		if (states[(int)State.Sprint] > 0)
			impulse *= maxVelocity;
		else
			impulse *= minVelocity;
		
		if (states[(int)State.Jump] == (int)StateStatus.Begin)
		{
			rigid.Frozen = false;
			impulse += z * MathLib.Fsqrt(2.0f * 9.8f * jumping) / (acceleration * ifps);
		}
		
		if (Ground > 0)
			rigid.LinearVelocity = x * xVelocity + y * yVelocity + z * zVelocity;
		
		var targetVelocity = MathLib.Length(new vec2(MathLib.Dot(x, impulse), MathLib.Dot(y, impulse)));
		
		velocity = rigid.LinearVelocity;
		var oldVelocity = MathLib.Length(new vec2(MathLib.Dot(x, velocity), MathLib.Dot(y, velocity)));
		
		rigid.AddLinearImpulse(impulse * (acceleration * ifps * shape.Mass));
		
		var currentVelocity = MathLib.Length(new vec2(MathLib.Dot(x, rigid.LinearVelocity), MathLib.Dot(y, rigid.LinearVelocity)));
		if (targetVelocity < MathLib.EPSILON || currentVelocity > targetVelocity)
		{
			vec3 linearVelocity = z * MathLib.Dot(z, rigid.LinearVelocity);
			linearVelocity += (x * MathLib.Dot(x, rigid.LinearVelocity) + y * MathLib.Dot(y, rigid.LinearVelocity)) * MathLib.Exp(-damping * ifps);
			rigid.LinearVelocity = linearVelocity;
		}
		
		currentVelocity = MathLib.Length(new vec2(MathLib.Dot(x, rigid.LinearVelocity), MathLib.Dot(y, rigid.LinearVelocity)));
		if (currentVelocity > oldVelocity)
		{
			if (currentVelocity > targetVelocity)
			{
				var linearVelocity = z * MathLib.Dot(z, rigid.LinearVelocity);
				linearVelocity += (x * MathLib.Dot(x, rigid.LinearVelocity) + y * MathLib.Dot(y, rigid.LinearVelocity)) * targetVelocity / currentVelocity;
				rigid.LinearVelocity = linearVelocity;
			}
		}
		
		if (currentVelocity < MathLib.EPSILON)
			rigid.LinearVelocity = z * MathLib.Dot(z, rigid.LinearVelocity);
		
		if (targetVelocity < MathLib.EPSILON)
			shape.Friction = maxFriction;
		else
			shape.Friction = minFriction;
		
		Ground = 0;
		Ceiling = 0;
		
		var cap0 = shape.BottomCap;
		var cap1 = shape.TopCap;
		for (int i = 0; i < rigid.NumContacts; i++)
		{
			var point = rigid.GetContactPoint(i);
			var normal = rigid.GetContactNormal(i);
			if (MathLib.Dot(normal, up) > 0.5f && MathLib.Dot(new vec3(point - cap0), up) < 0.0f)
				Ground = 1;
			if (MathLib.Dot(normal, up) < -0.5f && MathLib.Dot(new vec3(point - cap1), up) > 0.0f)
				Ceiling = 1;
		}
		position = objectDummy.WorldTransform.GetColumn3(3);

		FlushTransform();
	}
	private void UpdateTransform()
	{
		var up = vec3.UP;
		vec3 tangent, binormal;
		Geometry.OrthoBasis(up, out tangent, out binormal);

		position = node.WorldTransform.GetColumn3(3);
		direction = MathLib.Normalize(new vec3(node.WorldTransform.GetColumn3(1)));

		phiAngle = MathLib.Atan2(MathLib.Dot(direction, tangent), MathLib.Dot(direction, binormal)) * MathLib.RAD2DEG;

		objectDummy.WorldTransform = GetBodyTransform();
	}
	private void FlushTransform()
	{
		var up = vec3.UP;
		node.WorldTransform = MathLib.SetTo(position, position + new vec3(direction - up * MathLib.Dot(direction, up)), up) * new mat4(MathLib.RotateX(-90.0f));
	}
}