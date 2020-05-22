#ifndef __TRACKER_WRAPPER_H__
#define __TRACKER_WRAPPER_H__

#include <core/unigine.h>
#include <core/systems/tracker/tracker.h>

namespace TrackerWrapper
{
	using Unigine::Tracker;
	
	Tracker tracker;			// tracker
	TrackerTrack track;			// main track
	float track_keys[0];		// track keys

	void init(string file_name)
	{
		// load clip track
		tracker = new Tracker();
		track = tracker.loadTrack(file_name);
		assert(track.getNumParameterKeys(0) > 0 && "TrackerWrapper::init(): can't load clip track");
		track.getParameterKeys(0,track_keys);
	}
	
	void shutdown() 
	{
		delete track;
		delete tracker;
		track_keys.clear();
	}
	
	float getMinTime()
	{
		return track.getMinTime();
	}
	
	float getMaxTime()
	{
		return track.getMaxTime();
	}
	
	float getUnitTime()
	{
		return track.getUnitTime();
	}
	
	void set(float time)
	{
		track.set(time);
	}
	
	int getKeysLength()
	{
		return track_keys.size();
	}
	
	float getKey(int num)
	{
		return track_keys[num];
	}
	
	int getRightKey(float time)
	{
		return track_keys.right(time);
	}
	
	int getLeftKey(float time)
	{
		return track_keys.left(time);
	}
}

#endif