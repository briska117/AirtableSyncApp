// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableRecord`1
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using Newtonsoft.Json;
using System;

namespace AirtableApiClient
{
  public class AirtableRecord<T>
  {
    [JsonProperty("id")]
    public string Id { get; internal set; }

    [JsonProperty("createdTime")]
    public DateTime CreatedTime { get; internal set; }

    [JsonProperty("fields")]
    public T Fields { get; internal set; }
  }
}
