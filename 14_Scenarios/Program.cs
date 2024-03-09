#define P1A
// #define P1B
// #define P1C
// #define P1D
// #define P1E
// #define P3B
// #define P4A
// #define P4B
// #define P5

using _14_Scenarios;

#if P1A
P1A p1a = new P1A();
p1a.UseSharedInteger();
#endif

#if P1B
P1B p1b = new P1B();
await p1b.GetSharedIntegerAsync();
#endif

#if P1C
P1C p1c = new P1C();
await p1c.GetSharedIntegerAsync();
#endif

#if P1D
P1D p1d = new P1D();
await p1d.GetSharedIntegerAsync();
#endif

#if P3B
P3B.BindableTask<int> bindableTask = new P3B.BindableTask<int>(Task.FromResult(42));
#endif

#if P4A
P4A p4a = new P4A();
await p4a.DoLongOperationAsync();
#endif

#if P4B
P4B p4b = new P4B();
await p4b.DoLongOperationAsync();
#endif

#if P5
P5 p5 = new P5();
int resultSync = p5.DelayAndReturn();
Task<int> resultAsync = p5.DelayAndReturnAsync();
#endif