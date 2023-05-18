// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtablePaymentRequiredException
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Net;

namespace AirtableApiClient
{
  public class AirtablePaymentRequiredException : AirtableApiException
  {
    public AirtablePaymentRequiredException()
      : base(HttpStatusCode.PaymentRequired, "Payment Required", "The account associated with the API key making requests hits a quota that can be increased by upgrading the Airtable account plan.")
    {
    }
  }
}
