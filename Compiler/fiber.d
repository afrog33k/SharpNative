import core.thread, std.stdio;
void fun() { writeln("A"); Fiber.yield(); writeln("B"); }
void main() {
   auto f = new Fiber(&fun);
   writeln("call..");
   f.call(false);
   writeln("call again..");
   f.call(false);
   writeln("exit");
}
