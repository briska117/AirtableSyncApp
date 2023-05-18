// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.Fields
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace AirtableApiClient
{
  public class Fields
  {
    [JsonProperty("fields")]
    public Dictionary<string, object> FieldsCollection { get; set; } = new Dictionary<string, object>();

    public void AddField(string fieldName, object fieldValue) => this.FieldsCollection.Add(fieldName, fieldValue);
  }
}
