#define P1
// #define P2
// #define P3
// #define P4
// #define P7A
// #define P7B

using System.Net;
using _8_Interop;

WebClient client = new WebClient();
const string url = "https://t.me";

#if P1
Uri address = new Uri(url);
string resultP1 = await P1.DownloadStringTaskAsync(client, address);
Console.WriteLine(resultP1);
#endif

#if P2
WebRequest webRequest = WebRequest.Create(url);
WebResponse resultP2 = await P2.GetResponseAsync(webRequest);
Console.WriteLine(resultP2);
#endif

#if P3
P3.IMyAsyncHttpService httpService = new MyAsyncHttpService();
Uri addressP3 = new Uri(url);
string resultP3 = await P3.DownloadStringAsync(httpService, addressP3);
Console.WriteLine(resultP3);
#endif

#if P4
P4 p4 = new P4();
await p4.Test();
#endif

#if P7A
P7A p7A = new P7A();
await p7A.Test();
#endif

public class MyAsyncHttpService : P3.IMyAsyncHttpService
{
    public void DownloadString(Uri address, Action<string, Exception> callback)
    {
        // Реализация метода
    }
}