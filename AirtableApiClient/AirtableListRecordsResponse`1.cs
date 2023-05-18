// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableListRecordsResponse`1
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using System.Collections.Generic;

namespace AirtableApiClient
{
  public class AirtableListRecordsResponse<T> : AirtableApiResponse
  {
    public readonly IEnumerable<AirtableRecord<T>> Records;
    public readonly string Offset;

    public AirtableListRecordsResponse(AirtableApiException error)
      : base(error)
    {
      this.Offset = (string) null;
      this.Records = (IEnumerable<AirtableRecord<T>>) null;
    }

    public AirtableListRecordsResponse(AirtableRecordList<T> recordList)
    {
      this.Offset = recordList.Offset;
      this.Records = (IEnumerable<AirtableRecord<T>>) recordList.Records;
    }
  }
}
