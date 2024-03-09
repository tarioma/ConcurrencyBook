#define P1
// #define P2
// #define P3
// #define P4
// #define P5
// #define P6

using System.Threading.Tasks.Dataflow;
using _5_DataflowBasics;

#if P1
Console.WriteLine("\nTesting P1:");
var p1 = new P1();
p1.Test();
await p1.TestB();
#endif

#if P2
Console.WriteLine("\nTesting P2:");
var p2 = new P2();
p2.Test();
await p2.Test2();
await p2.Test3();
#endif

#if P3
Console.WriteLine("\nTesting P3:");
var p3 = new P3();
p3.Test();
#endif

#if P4
Console.WriteLine("\nTesting P4:");
var p4 = new P4();
p4.Test();
#endif

#if P5
Console.WriteLine("\nTesting P5:");
var p5 = new P5();
p5.Test();
#endif

#if P6
Console.WriteLine("\nTesting P6:");
var p6 = new P6();
var customBlock = p6.CreateMyCustomBlock();
customBlock.Post(5);
var result = await customBlock.ReceiveAsync();
Console.WriteLine($"Result from custom block: {result}");
#endif