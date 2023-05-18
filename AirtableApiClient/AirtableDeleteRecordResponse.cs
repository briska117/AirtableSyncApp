// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableDeleteRecordResponse
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

namespace AirtableApiClient
{
  public class AirtableDeleteRecordResponse : AirtableApiResponse
  {
    public readonly bool Deleted;
    public readonly string Id;

    public AirtableDeleteRecordResponse(AirtableApiException error)
      : base(error)
    {
      this.Deleted = false;
      this.Id = (string) null;
    }

    public AirtableDeleteRecordResponse(bool deleted, string id)
    {
      this.Deleted = deleted;
      this.Id = id;
    }
  }
}
