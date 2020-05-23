using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using Console = Unigine.Console;

[Component(PropertyGuid = "eb422059b05f0de46c0f7ff364ee44b601aef49d")]
public class SimpleComponent : Component
{
	private void Init()
	{
		// write here code to be called on component initialization
	}
	
	private void Update()
	{
		// write here code to be called before updating each render frame
		OnFunction(Game.Time);
	}

	private void OnFunction(float test)
	{
		Log.Message(test.ToString());
	}
}