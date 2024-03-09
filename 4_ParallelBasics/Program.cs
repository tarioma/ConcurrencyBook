#define P2A
// #define P2B
// #define P2C
// #define P3
// #define P4B
// #define P5A
// #define P5B

using _4_ParallelBasics;

#if P2A
Console.WriteLine("\nTesting P2A:");
var p2A = new P2A();
var valuesForP2A = Enumerable.Range(1, 10);
var sumP2A = p2A.ParallelSum(valuesForP2A);
Console.WriteLine($"Parallel Sum: {sumP2A}");
#endif

#if P2B
Console.WriteLine("\nTesting P2B:");
var p2B = new P2B();
var valuesForP2B = Enumerable.Range(1, 10);
var sumP2B = p2B.ParallelSum(valuesForP2B);
Console.WriteLine($"Parallel Sum: {sumP2B}");
#endif

#if P2C
Console.WriteLine("\nTesting P2C:");
var p2C = new P2C();
var valuesForP2C = Enumerable.Range(1, 10);
var sumP2C = p2C.ParallelSum(valuesForP2C);
Console.WriteLine($"Parallel Sum: {sumP2C}");
#endif

#if P3
Console.WriteLine("\nTesting P3:");
var p3 = new P3();
var arrayForP3 = new double[1000];
p3.ProcessArray(arrayForP3);

var actionForP3 = new Action(() => Console.WriteLine("Executing action"));
p3.DoAction20Times(actionForP3);

var tokenSourceP3 = new CancellationTokenSource();
p3.DoAction20Times(actionForP3, tokenSourceP3.Token);
#endif

#if P4B
Console.WriteLine("\nTesting P4B:");
var p4B = new P4B();
p4B.Test();
#endif

#if P5A
Console.WriteLine("\nTesting P5A:");
var p5A = new P5A();
var valuesForP5A = Enumerable.Range(1, 5);
var resultP5A = p5A.MultiplyBy2(valuesForP5A);
Console.WriteLine($"Result: {string.Join(", ", resultP5A)}");
#endif

#if P5B
Console.WriteLine("\nTesting P5B:");
var p5B = new P5B();
var valuesForP5B = Enumerable.Range(1, 5);
var resultP5B = p5B.MultiplyBy2(valuesForP5B);
Console.WriteLine($"Result: {string.Join(", ", resultP5B)}");

var sumP5B = p5B.ParallelSum(valuesForP5B);
Console.WriteLine($"Parallel Sum: {sumP5B}");
#endif