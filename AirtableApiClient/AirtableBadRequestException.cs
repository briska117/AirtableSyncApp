// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableBadRequestException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableBadRequestException : AirtableApiException
  {
    public AirtableBadRequestException()
      : base(HttpStatusCode.BadRequest, "Bad Request", "The request encoding is invalid; the request can't be parsed as a valid JSON.")
    {
    }
  }
}
