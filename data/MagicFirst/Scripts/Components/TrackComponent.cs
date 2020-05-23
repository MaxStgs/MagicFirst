using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unigine;
using Unigine.Plugins;
using Component = Unigine.Component;

[Component(PropertyGuid = "9a98ef7c68ba84abd376d20ed4873215c94ddc70")]
public class TrackComponent : Component
{
	[ShowInEditor, ParameterFile(Filter = ".track")]
	private readonly AssetLink track = null;
	
	private void Init()
	{
		// write here code to be called on component initialization
		if (track == null) return;
		
		Engine.RunWorldFunction(new Variable("TrackerWrapper::init"),
			new Variable(track.Path));
		
	}

	private float time = 0.0f;
	
	private void Update()
	{
		// write here code to be called before updating each render frame
		if (track == null) return;
		
		Engine.RunWorldFunction(new Variable("TrackerWrapper::set"), new Variable(time));
		time += Game.IFps;
	}
}