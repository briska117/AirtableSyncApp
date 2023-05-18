// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableUnrecognizedException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableUnrecognizedException : AirtableApiException
  {
    public AirtableUnrecognizedException(HttpStatusCode statusCode)
      : base(statusCode, "Unrecognized Error", string.Format("Airtable returned HTTP status code {0}", (object) statusCode))
    {
    }
  }
}
