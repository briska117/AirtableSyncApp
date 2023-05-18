// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableRecordList`1
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using Newtonsoft.Json;

namespace AirtableApiClient
{
  public class AirtableRecordList<T>
  {
    [JsonProperty("offset")]
    public string Offset { get; internal set; }

    [JsonProperty("records")]
    public AirtableRecord<T>[] Records { get; internal set; }
  }
}
