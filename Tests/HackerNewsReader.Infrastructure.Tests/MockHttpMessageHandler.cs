public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _sendAsync;

    public MockHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendAsync)
    {
        _sendAsync = sendAsync ?? throw new ArgumentNullException(nameof(sendAsync));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return _sendAsync(request);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error in MockHttpMessageHandler.", ex);
        }
    }
}
