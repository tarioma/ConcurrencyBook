#define P1
// #define P2A
// #define P2B
// #define P2C
// #define P4A
// #define P4B
// #define P4C
// #define P4D

using _3_AsynchronousStreams;

var client = new HttpClient();

#if P1
Console.WriteLine("Testing P1:");
var p1 = new P1();
await foreach (var value in p1.GetValuesAsync())
{
    Console.WriteLine(value);
}

await foreach (var value in p1.GetValuesAsync(client))
{
    Console.WriteLine(value);
}
#endif

#if P2A
Console.WriteLine("\nTesting P2A:");
var p2A = new P2AImplementation();
await p2A.ProcessValueAsync(client);
#endif

#if P2B
Console.WriteLine("\nTesting P2B:");
var p2B = new P2BImplementation();
await p2B.ProcessValueAsync(client);
#endif

#if P2C
Console.WriteLine("\nTesting P2C:");
var p2C = new P2CImplementation();
await p2C.ProcessValueAsync(client);
#endif

#if P4A
Console.WriteLine("\nTesting P4A:");
var p4A = new P4A();
await p4A.Test();
#endif

#if P4B
Console.WriteLine("\nTesting P4B:");
var p4B = new P4B();
await p4B.Test2();
#endif

#if P4C
Console.WriteLine("\nTesting P4C:");
var p4C = new P4C();
await p4C.ConsumeSequence(p4C.SlowRange());
#endif

#if P4D
Console.WriteLine("\nTesting P4D:");
var p4D = new P4D();
await p4D.ConsumeSequence(p4C.SlowRange());
#endif

class P2AImplementation : P2A
{
    public override async IAsyncEnumerable<string> GetValuesAsync(HttpClient client)
    {
        yield return "Value1";
        await Task.Delay(100);
        yield return "Value2";
    }
}

class P2BImplementation : P2B
{
    public override async IAsyncEnumerable<string> GetValuesAsync(HttpClient client)
    {
        yield return "Value1";
        await Task.Delay(100);
        yield return "Value2";
    }
}

class P2CImplementation : P2C
{
    public override async IAsyncEnumerable<string> GetValuesAsync(HttpClient client)
    {
        yield return "Value1";
        await Task.Delay(100);
        yield return "Value2";
    }
}