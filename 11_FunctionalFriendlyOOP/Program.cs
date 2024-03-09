#define P1
// #define P2A
// #define P2B
// #define P3A
// #define P4A
// #define P4B
// #define P4C
// #define P6A
// #define P6C

using _11_FunctionalFriendlyOOP;

#if P1
HttpClient client = new HttpClient();
P1.MyAsyncClass service = new P1.MyAsyncClass();

P1 p1 = new P1();
await p1.UseMyInterfaceAsync(client, service);
#endif

#if P2A
P2A p2a = new P2A();
await p2a.Test();
#endif

#if P2B
P2B p2b = new P2B();
await p2b.Test();
#endif

#if P3A
P3A p3a = new P3A();
await p3a.Test1();
#endif

#if P4A
P4A p4a = new P4A();
int result = await p4a.GetDataAsync();
Console.WriteLine($"Result of P4A: {result}");
#endif

#if P4B
P4B p4b = new P4B();
result = await p4b.Data;
Console.WriteLine($"Result of P4B: {result}");
#endif

#if P4C
P4C p4c = new P4C();
await p4c.Test();
#endif

#if P6A
P6A.MyClass myClassA = new P6A.MyClass();
result = await myClassA.CalculateValueAsync();
Console.WriteLine($"Result of P6A: {result}");
#endif

#if P6C
P6C p6c = new P6C();
await p6c.Test();
#endif