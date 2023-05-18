// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableCreateUpdateReplaceRecordResponse
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

namespace AirtableApiClient
{
  public class AirtableCreateUpdateReplaceRecordResponse : AirtableApiResponse
  {
    public readonly AirtableRecord Record;

    public AirtableCreateUpdateReplaceRecordResponse(AirtableApiException error)
      : base(error)
    {
      this.Record = (AirtableRecord) null;
    }

    public AirtableCreateUpdateReplaceRecordResponse(AirtableRecord record) => this.Record = record;
  }
}
