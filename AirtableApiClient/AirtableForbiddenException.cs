// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableForbiddenException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableForbiddenException : AirtableApiException
  {
    public AirtableForbiddenException()
      : base(HttpStatusCode.Forbidden, "Forbidden", "Accessing a protected resource with API credentials that don't have access to that resource.")
    {
    }
  }
}
