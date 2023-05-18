// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.Thumbnail
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using Newtonsoft.Json;

namespace AirtableApiClient
{
  public class Thumbnail
  {
    [JsonProperty("url")]
    public string Url { get; internal set; }

    [JsonProperty("width")]
    public long Width { get; internal set; }

    [JsonProperty("height")]
    public long Height { get; internal set; }
  }
}
