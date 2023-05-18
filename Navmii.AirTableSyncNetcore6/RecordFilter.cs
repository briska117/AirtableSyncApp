using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Newtonsoft.Json;

namespace Navmii.AirtableSync
{
    public abstract class RecordFilter
    {
        public abstract bool Check(AirtableRecord record);
    }

    public class TSheetsRecordFilter : RecordFilter
    {
        private readonly string teamName;

        public TSheetsRecordFilter(string teamName)
        {
            this.teamName = teamName;
        }

        public override bool Check(AirtableRecord record)
        {
            string group = "" + record.GetField("Group");
            return group.Trim().ToLower() == teamName.Trim().ToLower();
        }
    }

    public class TeamRecordFilter : RecordFilter
    {
        private readonly string teamFieldName;
        private readonly string teamID;

        public TeamRecordFilter(string teamID, string teamFieldName)
        {
            this.teamID = teamID;
            this.teamFieldName = teamFieldName;
        }

        public override bool Check(AirtableRecord record)
        {
            string json = "" + record.GetField(teamFieldName);
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            string[] teamIDs = JsonConvert.DeserializeObject<string[]>(json);
            if (teamIDs == null)
            {
                return false;
            }

            return teamIDs.Contains(teamID);
        }
    }
}
