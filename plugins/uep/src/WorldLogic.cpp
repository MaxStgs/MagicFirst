#include "WorldLogic.h"

#include <UnigineConsole.h>
#include <UnigineInput.h>
#include <UnigineApp.h>
#include <UnigineControls.h>
#include <UnigineEditor.h>
#include <Windows.h>

using namespace Unigine;

int my_world_logic::init()
{
    if (!Editor::isLoaded())
    {
        take_focus();
    }
    Log::message("uep MyWorldLogic::init()\n");
    return 1;
}

int my_world_logic::update()
{
    check_should_reload_world();
    check_have_focus();
    return 1;
}

void my_world_logic::check_should_reload_world()
{
    if (Input::isKeyPressed(Input::KEY_CTRL)
        && Input::isKeyPressed(Input::KEY_SHIFT)
        && Input::isKeyPressed(Input::KEY_R))
    {
        Console::run("world_reload");
    }
    if (Input::isKeyPressed(Input::KEY_R))
    {
        Log::message("Ctrl pressed!\n");
    }
}

void my_world_logic::take_focus() const
{
    App::setMouseGrab(true);
    ControlsApp::setMouseHandle(Input::MOUSE_HANDLE_GRAB);
    ControlsApp::setMouseEnabled(true);
    if (debug_)
    {
        Log::message("active\n");
    }
}

void my_world_logic::unfocus() const
{
    App::setMouseGrab(false);
    ControlsApp::setMouseHandle(Input::MOUSE_HANDLE_USER);
    ControlsApp::setMouseEnabled(false);
    if (debug_)
    {
        Log::message("is not active\n");
    }
}

void my_world_logic::check_have_focus()
{
    if (Editor::isLoaded()) return;
    
    const bool is_in_focus = is_app_in_focus();

    if (is_in_focus == is_last_focused_) return;
    
    if (is_in_focus)
    {
        is_last_focused_ = true;
        take_focus();
    }
    else
    {
        is_last_focused_ = false;
        unfocus();
    }
}

bool my_world_logic::is_app_in_focus()
{
    DWORD activePID;
    GetWindowThreadProcessId(GetForegroundWindow(), &activePID);
    
    return activePID == GetCurrentProcessId();
}


int my_world_logic::shutdown()
{
    Log::message("reload_world_plugin MyWorldLogic::shutdown()\n");

    return 1;
}
