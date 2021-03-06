﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestSama
{
    public abstract class TestHttpHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await RealSendAsync(request, cancellationToken);
        }

        public abstract Task<HttpResponseMessage> RealSendAsync(HttpRequestMessage request, CancellationToken cancellationToken);

        protected override void Dispose(bool disposing)
        {
            // Do nothing.
        }
    }
}
