// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableAttachment
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using Newtonsoft.Json;

namespace AirtableApiClient
{
  public class AirtableAttachment
  {
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("size")]
    public long? Size { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("thumbnails")]
    public Thumbnails Thumbnails { get; set; }
  }
}
