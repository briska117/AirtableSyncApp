// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableApiException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System;
using System.Net;

namespace AirtableApiClient
{
  public abstract class AirtableApiException : Exception
  {
    public readonly HttpStatusCode ErrorCode;
    public readonly string ErrorName;
    public readonly string ErrorMessage;

    protected AirtableApiException(HttpStatusCode errorCode, string errorName, string errorMessage)
      : base(string.Format("{0} - {1}: {2}", (object) errorName, (object) errorCode, (object) errorMessage))
    {
      this.ErrorCode = errorCode;
      this.ErrorName = errorName;
      this.ErrorMessage = errorMessage;
    }
  }
}
