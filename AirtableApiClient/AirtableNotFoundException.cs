// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableNotFoundException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableNotFoundException : AirtableApiException
  {
    public AirtableNotFoundException()
      : base(HttpStatusCode.NotFound, "Not Found", "Route or resource is not found. This error is returned when the request hits an undefined route, or if the resource doesn't exist (e.g. has been deleted).")
    {
    }
  }
}
