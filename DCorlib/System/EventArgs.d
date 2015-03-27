module System.EventArgs;

import System.Namespace;

class EventArgs {
    NObject source;
    bool handled;

    void handle() { handled = true; }

    this() {}
    this(NObject s) {
        source = s;
    }
}
