// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableUnauthorizedException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableUnauthorizedException : AirtableApiException
  {
    public AirtableUnauthorizedException()
      : base(HttpStatusCode.Unauthorized, "Unauthorized", "Accessing a protected resource without authorization or with invalid credentials.")
    {
    }
  }
}
