
namespace Electrify.Server.UnitTests.Substitutes;

public class SubstituteHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendAsyncFunc)
    : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return sendAsyncFunc(request);
    }
}
