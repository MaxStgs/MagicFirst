#pragma once
#include <UniginePlugin.h>

#include "WorldLogic.h"

using namespace Unigine;

class uep final : public Plugin
{
public:
    uep()
    {
        Log::message("MyPlugin::MyPlugin(): called\n");
    }

    virtual ~uep()
    {
        Log::message("MyPlugin::~MyPlugin(): called\n");
    }

    // uep data
    void* get_data() override
    {
        return this;
    }

    // initialize uep
    int init() override;

    // shutdown uep
    int shutdown() override;

    // destroy uep
    void destroyRenderResources() override;

private:
    my_world_logic world_logic_;
};
