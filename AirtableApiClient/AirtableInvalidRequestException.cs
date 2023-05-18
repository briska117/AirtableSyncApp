// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableInvalidRequestException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtableInvalidRequestException : AirtableApiException
  {
    public string DetailedErrorMessage { get; }

    public AirtableInvalidRequestException(string errorMessage = null)
      : base((HttpStatusCode) 422, "Invalid Request", "The request data is invalid. This includes most of the base-specific validations. The DetailedErrorMessage property contains the detailed error message string.")
    {
      this.DetailedErrorMessage = errorMessage;
    }
  }
}
