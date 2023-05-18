// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableCreateUpdateReplaceMultipleRecordsResponse
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

namespace AirtableApiClient
{
  public class AirtableCreateUpdateReplaceMultipleRecordsResponse : AirtableApiResponse
  {
    public readonly AirtableRecord[] Records;

    public AirtableCreateUpdateReplaceMultipleRecordsResponse(AirtableApiException error)
      : base(error)
    {
      this.Records = (AirtableRecord[]) null;
    }

    public AirtableCreateUpdateReplaceMultipleRecordsResponse(AirtableRecord[] records) => this.Records = records;
  }
}
