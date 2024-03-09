#define P1
// #define P2A
// #define P2B
// #define P3A
// #define P3B
// #define P4
// #define P5
// #define P7
// #define P8
// #define P9

using System.Net.NetworkInformation;
using _10_Cancellation;

#if P1
P1 p1Instance = new P1();
p1Instance.IssueCancelRequest();
await p1Instance.IssueCancelRequestAsync();
#endif

#if P2A
P2A p2aInstance = new P2A();
CancellationTokenSource ctsP2A = new CancellationTokenSource();
Task.Run(() => p2aInstance.CancelableMethod(ctsP2A.Token), ctsP2A.Token);
#endif

#if P2B
P2B p2bInstance = new P2B();
CancellationTokenSource ctsP2B = new CancellationTokenSource();
Task.Run(() => p2bInstance.CancelableMethod(ctsP2B.Token), ctsP2B.Token);
#endif

#if P3A
P3A p3aInstance = new P3A();
await p3aInstance.IssueTimeoutAsync();
#endif

#if P3B
P3B p3bInstance = new P3B();
await p3bInstance.IssueTimeoutAsync();
#endif

#if P4
P4 p4Instance = new P4();
CancellationTokenSource ctsP4 = new CancellationTokenSource();
Task<int> cancelableMethodTask = p4Instance.CancelableMethodAsync(ctsP4.Token);
ctsP4.Cancel();
try
{
    int result = await cancelableMethodTask;
    Console.WriteLine($"P4 Result: {result}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("P4 Operation Canceled");
}
#endif

#if P5
P5 p5Instance = new P5();
IEnumerable<int> values = Enumerable.Range(1, 10);
CancellationTokenSource ctsP5 = new CancellationTokenSource();
IEnumerable<int> multipliedValues = p5Instance.MultiplyBy2(values, ctsP5.Token);
foreach (var item in multipliedValues)
{
    Console.Write($"{item} ");
}
Console.WriteLine();
#endif

#if P8
P8 p8Instance = new P8();
HttpClient httpClient = new HttpClient();
CancellationTokenSource ctsP8 = new CancellationTokenSource();
string url = "https://www.example.com";
try
{
    HttpResponseMessage response = await p8Instance.GetWithTimeoutAsync(httpClient, url, ctsP8.Token);
    Console.WriteLine($"P8 Response: {response.StatusCode}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("P8 Operation Canceled");
}
#endif

#if P9
P9 p9Instance = new P9();
string hostName = "www.google.com";
CancellationTokenSource ctsP9 = new CancellationTokenSource();
try
{
    PingReply pingReply = await p9Instance.PingAsync(hostName, ctsP9.Token);
    Console.WriteLine($"P9 Roundtrip Time: {pingReply.RoundtripTime}ms");
}
catch (OperationCanceledException)
{
    Console.WriteLine("P9 Ping Canceled");
}
#endif