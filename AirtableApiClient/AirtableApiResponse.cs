// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableApiResponse
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

namespace AirtableApiClient
{
  public abstract class AirtableApiResponse
  {
    public readonly bool Success;
    public readonly AirtableApiException AirtableApiError;

    protected AirtableApiResponse()
    {
      this.Success = true;
      this.AirtableApiError = (AirtableApiException) null;
    }

    protected AirtableApiResponse(AirtableApiException error)
    {
      this.Success = false;
      this.AirtableApiError = error;
    }
  }
}
