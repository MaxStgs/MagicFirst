/* Copyright (C) 2005-2020, UNIGINE. All rights reserved.
 *
 * This file is a part of the UNIGINE 2.11.0.1 SDK.
 *
 * Your use and / or redistribution of this software in source and / or
 * binary form, with or without modification, is subject to: (i) your
 * ongoing acceptance of and compliance with the terms and conditions of
 * the UNIGINE License Agreement; and (ii) your inclusion of this notice
 * in any version of this software that you use or redistribute.
 * A copy of the UNIGINE License Agreement is available by contacting
 * UNIGINE. at http://unigine.com/
 */


#include "uep.h"

#include <UnigineInterface.h>
#include <UnigineLogic.h>
#include <UnigineConsole.h>

using namespace Unigine;

int uep::init()
{
    Log::message("reload_world_plugin::init()\n");

    // add C++ callbacks
    Engine* engine = Engine::get();
    engine->addWorldLogic(&world_logic_);

    return 1;
}

int uep::shutdown()
{
    Log::message("reload_world_plugin::shutdown()\n");

    // remove interpreter resources
    Interpreter::removeGroup("MyPlugin");

    // remove C++ callbacks
    Engine* engine = Engine::get();
    engine->removeWorldLogic(&world_logic_);

    return 1;
}

void uep::destroyRenderResources()
{
    Log::message("reload_world_plugin::destroyRenderResources(): called\n");
}

extern "C" UNIGINE_EXPORT void* CreatePlugin()
{
    Log::message("CreatePlugin called\n");
    return new uep();
}

extern "C" UNIGINE_EXPORT void ReleasePlugin(void* plugin)
{
    delete static_cast<uep*>(plugin);
}
