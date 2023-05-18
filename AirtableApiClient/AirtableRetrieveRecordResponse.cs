// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableRetrieveRecordResponse
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

namespace AirtableApiClient
{
  public class AirtableRetrieveRecordResponse : AirtableApiResponse
  {
    public readonly AirtableRecord Record;

    public AirtableRetrieveRecordResponse(AirtableApiException error)
      : base(error)
    {
      this.Record = (AirtableRecord) null;
    }

    public AirtableRetrieveRecordResponse(AirtableRecord record) => this.Record = record;
  }
}
