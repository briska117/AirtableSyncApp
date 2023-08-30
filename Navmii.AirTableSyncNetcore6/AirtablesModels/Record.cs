using AirtableApiClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class Record
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }

        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; internal set; }

        [JsonProperty("fields")]
        public Dictionary<string, object> Fields { get; internal set; } = new Dictionary<string, object>();


        public object GetField(string fieldName)
        {
            if (Fields.ContainsKey(fieldName))
            {
                return Fields[fieldName];
            }

            return null;
        }

        public IEnumerable<AirtableAttachment> GetAttachmentField(string attachmentsFieldName)
        {
            object field = GetField(attachmentsFieldName);
            if (field == null)
            {
                return null;
            }

            List<AirtableAttachment> list = new List<AirtableAttachment>();
            try
            {
                string value = JsonConvert.SerializeObject(field);
                foreach (Dictionary<string, object> item in JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(value)!)
                {
                    value = JsonConvert.SerializeObject(item);
                    list.Add(JsonConvert.DeserializeObject<AirtableAttachment>(value));
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Field '" + attachmentsFieldName + "' is not an Attachments field." + Environment.NewLine + "It has caused the exception: " + ex.Message);
            }
        }
    }
}
