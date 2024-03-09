#define P1
// #define P2A
// #define P2B
// #define P3
// #define P4
// #define P5
// #define P6A
// #define P6B
// #define P6C
// #define P7
// #define P8A
// #define P8B
// #define P9A
// #define P10A
// #define P10C
// #define P11A
// #define P11B
// #define P11C
// #define P11D

using _2_AsyncBasics;

var client = new HttpClient();
const string url = "https://t.me";

#if P1
var result1 = await new P1().DownloadStringWithRetries(client, url);
Console.WriteLine($"P1: {result1}");

var result2 = await new P1().DownloadStringWithTimeout(client, url);
Console.WriteLine($"P1: {result2}");
#endif

#if P2A
var p2A = new P2A.MySynchronousImplementation();
Console.WriteLine($"P2A: {await p2A.GetValueAsync()}");
#endif

#if P2B
var p2BInstance = new P2B.MySynchronousImplementation();
await p2BInstance.DoSomethingAsync();
#endif

#if P3
var p3 = new P3();
var progress = new Progress<double>();
progress.ProgressChanged += (_, args) =>
{
    // Handle progress change
    Console.WriteLine($"Progress changed: {args}");
};

await p3.MyMethodAsync(progress);
await p3.CallMyMethodAsync();
#endif

#if P4
var p4 = new P4();

// Test
await p4.Test();

// Test2
await p4.Test2();

// DownloadAllAsync
var urls = new List<string> { url, url, url };
string resultDownloadAll = await p4.DownloadAllAsync(client, urls);
Console.WriteLine($"DownloadAllAsync result: {resultDownloadAll}");

// ObserveOneExceptionAsync
try
{
    await p4.ObserveOneExceptionAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"ObserveOneExceptionAsync caught exception: {ex.GetType().Name}");
}

// ObserveAllExceptionsAsync
try
{
    await p4.ObserveAllExceptionsAsync();
}
catch
{
    Console.WriteLine("ObserveAllExceptionsAsync caught AggregateException");
}
#endif

#if P5
var p5 = new P5();

int result = await p5.FirstRespondingUrlAsync(client, url, url);
Console.WriteLine($"FirstRespondingUrlAsync result: {result}");
#endif

#if P6A
var p6A = new P6A();
await p6A.ProcessTasksAsync();
#endif

#if P6B
var p6B = new P6B();
await p6B.ProcessTasksAsync();
#endif

#if P6C
var p6C = new P6C();
await p6C.ProcessTasksAsync();
#endif

#if P7
var p7 = new P7();
await p7.ResumeOnContextAsync();
await p7.ResumeWithoutContextAsync();
#endif

#if P8A
var p8A = new P8A();
await p8A.TestAsync();
#endif

#if P8B
var p8B = new P8B();
await p8B.TestAsync();
#endif

#if P9A
var myAsyncCommand = new P9A.MyAsyncCommand();
await myAsyncCommand.Execute(null);
#endif

#if P10A
var p10A = new P10A();
int resultP10A = await p10A.MethodAsync();
Console.WriteLine($"P10A.MethodAsync result: {resultP10A}");
#endif

#if P10C
var p10C = new P10C();
await p10C.DisposeAsync();
#endif

#if P11A
var p11A = new P11A();
await p11A.ConsumingMethodAsync();
#endif

#if P11B
var p11B = new P11B();
await p11B.ConsumingMethodAsync();
#endif

#if P11C
var p11C = new P11C();
await p11C.ConsumingMethodAsync();
#endif

#if P11D
var p11D = new P11D();
await p11D.ConsumingMethodAsync();
#endif