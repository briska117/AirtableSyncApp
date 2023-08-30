using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class Options
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }

        [JsonProperty("referencedFieldIds")]
        public List<string> ReferencedFieldIds { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }

        [JsonProperty("linkedTableId")]
        public string LinkedTableId { get; set; }

        [JsonProperty("isReversed")]
        public bool? IsReversed { get; set; }

        [JsonProperty("prefersSingleRecordLink")]
        public bool? PrefersSingleRecordLink { get; set; }

        [JsonProperty("inverseLinkFieldId")]
        public string InverseLinkFieldId { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("precision")]
        public int? Precision { get; set; }

        [JsonProperty("viewIdForRecordSelection")]
        public string ViewIdForRecordSelection { get; set; }

        [JsonProperty("recordLinkFieldId")]
        public string RecordLinkFieldId { get; set; }

        [JsonProperty("dateFormat")]
        public DateFormat DateFormat { get; set; }

        [JsonProperty("fieldIdInLinkedTable")]
        public string FieldIdInLinkedTable { get; set; }

        [JsonProperty("durationFormat")]
        public string DurationFormat { get; set; }

        [JsonProperty("timeFormat")]
        public TimeFormat TimeFormat { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
