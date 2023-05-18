// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.HttpClientWithRetries
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AirtableApiClient
{
  internal class HttpClientWithRetries : IDisposable
  {
    public const int MAX_RETRIES = 3;
    public const int MIN_RETRY_DELAY_MILLISECONDS_IF_RATE_LIMITED = 2000;
    private int retryDelayMillisecondsIfRateLimited;
    private readonly HttpClient client;

    public bool ShouldNotRetryIfRateLimited { get; set; }

    public int RetryDelayMillisecondsIfRateLimited
    {
      get => this.retryDelayMillisecondsIfRateLimited;
      set => this.retryDelayMillisecondsIfRateLimited = value >= 2000 ? value : throw new ArgumentException(string.Format("Retry Delay cannot be less than {0} ms.", (object) 2000), "RetryDelayMilliSecondsIfRateLimited");
    }

    public HttpClientWithRetries(DelegatingHandler delegatingHandler, string apiKey)
    {
      this.ShouldNotRetryIfRateLimited = false;
      this.RetryDelayMillisecondsIfRateLimited = 2000;
      this.client = delegatingHandler != null ? new HttpClient((HttpMessageHandler) delegatingHandler) : new HttpClient();
      this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public void Dispose() => this.client.Dispose();

    public async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request)
    {
      string content = (string) null;
      if (request.Content != null)
        content = await request.Content.ReadAsStringAsync();
      int dueTimeDelay = this.RetryDelayMillisecondsIfRateLimited;
      int retries = 0;
      HttpResponseMessage httpResponseMessage = await this.client.SendAsync(request);
      while (httpResponseMessage.StatusCode == (HttpStatusCode) 429 && retries < 3 && !this.ShouldNotRetryIfRateLimited)
      {
        await Task.Delay(dueTimeDelay);
        httpResponseMessage = await this.client.SendAsync(this.RegenerateRequest(request.Method, request.RequestUri, content));
        ++retries;
        dueTimeDelay *= 2;
      }
      return httpResponseMessage;
    }

    private HttpRequestMessage RegenerateRequest(
      HttpMethod method,
      Uri requestUri,
      string content)
    {
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, requestUri);
      if (content != null)
        httpRequestMessage.Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json");
      return httpRequestMessage;
    }
  }
}
