#define P0A
// #define P0B
// #define P0C
// #define P0D
// #define P0E
// #define P0F
// #define P0G
// #define P0H
// #define P0I
// #define P1
// #define P2A
// #define P2B
// #define P3A
// #define P4A
// #define P4B
// #define P5

using _12_Synchronization;

#if P0A
P0A p0a = new P0A();
await p0a.MyMethodAsync();
#endif

#if P0B
P0B p0b = new P0B();
await p0b.ModifyValueConcurrentlyAsync();
#endif

#if P0C
P0C p0c = new P0C();
await p0c.ModifyValueConcurrentlyAsync();
#endif

#if P0D
P0D p0d = new P0D();
int resultP0D = await p0d.SimpleParallelismAsync();
Console.WriteLine($"Result of P0D: {resultP0D}");
#endif

#if P0E
IEnumerable<int> valuesP0E = Enumerable.Range(1, 10);
P0E p0e = new P0E();
p0e.IndependentParallelism(valuesP0E);
#endif

#if P0F
IEnumerable<int> valuesP0F = Enumerable.Range(1, 10);
P0F p0f = new P0F();
int resultP0F = p0f.ParallelSum(valuesP0F);
Console.WriteLine($"Result of P0F: {resultP0F}");
#endif

#if P0G
P0G p0g = new P0G();
bool isEmptyP0G = await p0g.PlayWithStackAsync();
Console.WriteLine($"Is stack empty in P0G: {isEmptyP0G}");
#endif

#if P0H
P0H p0h = new P0H();
bool isEmptyP0H = await p0h.PlayWithStackAsync();
Console.WriteLine($"Is stack empty in P0H: {isEmptyP0H}");
#endif

#if P0I
P0I p0i = new P0I();
int countP0I = await p0i.ThreadsafeCollectionsAsync();
Console.WriteLine($"Count of elements in dictionary in P0I: {countP0I}");
#endif

#if P1
P1.MyClass myClassP1 = new P1.MyClass();
myClassP1.Increment();
#endif

#if P2A
P2A.MyClass myClassP2A = new P2A.MyClass();
await myClassP2A.DelayAndIncrementAsync();
#endif

#if P2B
P2B.MyClass myClassP2B = new P2B.MyClass();
await myClassP2B.DelayAndIncrementAsync();
#endif

#if P3A
P3A.MyClass myClassP3A = new P3A.MyClass();
int resultP3A = myClassP3A.WaitForInitialization();
Console.WriteLine($"Result of P3A: {resultP3A}");
#endif

#if P4A
P4A.MyClass myClassP4A = new P4A.MyClass();
int resultP4A = await myClassP4A.WaitForInitializationAsync();
Console.WriteLine($"Result of P4A: {resultP4A}");
#endif

#if P4B
P4B.MyClass myClassP4B = new P4B.MyClass();
await myClassP4B.WaitForConnectedAsync();
#endif

#if P5
HttpClient client = new HttpClient();
IEnumerable<string> urlsP5 = new List<string> { "url1", "url2", "url3" };
P5 p5 = new P5();
string[] resultsP5 = await p5.DownloadUrlsAsync(client, urlsP5);
#endif