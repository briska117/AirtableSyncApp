// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableRequestEntityTooLargeException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableRequestEntityTooLargeException : AirtableApiException
  {
    public AirtableRequestEntityTooLargeException()
      : base(HttpStatusCode.RequestEntityTooLarge, "Request Entity Too Large", "The request exceeded the maximum allowed payload size. You shouldn't encounter this under normal use.")
    {
    }
  }
}
