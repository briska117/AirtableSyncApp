// Decompiled with JetBrains decompiler
// Type: AirtableApiClient.AirtableRecord
// Assembly: AirtableApiClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25C06998-048B-463F-91AD-21923E890255
// Assembly location: C:\Users\brian\Downloads\AirtableApiClient.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AirtableApiClient
{
  public class AirtableRecord
  {
    [JsonProperty("id")]
    public string Id { get; internal set; }

    [JsonProperty("createdTime")]
    public DateTime CreatedTime { get; internal set; }

    [JsonProperty("fields")]
    public Dictionary<string, object> Fields { get; internal set; } = new Dictionary<string, object>();

    public object GetField(string fieldName) => this.Fields.ContainsKey(fieldName) ? this.Fields[fieldName] : (object) null;

    public IEnumerable<AirtableAttachment> GetAttachmentField(
      string attachmentsFieldName)
    {
      object field = this.GetField(attachmentsFieldName);
      if (field == null)
        return (IEnumerable<AirtableAttachment>) null;
      List<AirtableAttachment> airtableAttachmentList = new List<AirtableAttachment>();
      try
      {
        foreach (object obj in JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(JsonConvert.SerializeObject(field)))
        {
          string str = JsonConvert.SerializeObject(obj);
          airtableAttachmentList.Add(JsonConvert.DeserializeObject<AirtableAttachment>(str));
        }
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Field '" + attachmentsFieldName + "' is not an Attachments field." + Environment.NewLine + "It has caused the exception: " + ex.Message);
      }
      return (IEnumerable<AirtableAttachment>) airtableAttachmentList;
    }
  }
}
