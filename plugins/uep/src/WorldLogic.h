#pragma once
#include <UnigineLogic.h>

class my_world_logic : public Unigine::WorldLogic
{

    bool is_last_focused_ = true;

    bool debug_ = false;
    
public:
    // initialize world
    int init() override;

    int update() override;

    static void check_should_reload_world();
    void take_focus() const;
    void unfocus() const;
    void check_have_focus();
    static bool is_app_in_focus();

    // shutdown world
    int shutdown() override;
};
