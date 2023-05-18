// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableTooManyRequestsException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableTooManyRequestsException : AirtableApiException
  {
    public AirtableTooManyRequestsException()
      : base((HttpStatusCode) 429, "Too Many Requests", "The user has sent too many requests in a given amount of time (rate limiting).")
    {
    }
  }
}
