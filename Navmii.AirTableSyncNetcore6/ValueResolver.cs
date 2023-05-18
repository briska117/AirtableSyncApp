using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Newtonsoft.Json;

namespace Navmii.AirtableSync
{
    public abstract class ValueResolver
    {
        public abstract object Resolve(object originalValue, Dictionary<string, object> originalFields);
    }

    public class DefaultLookupResolver : ValueResolver
    {
        private readonly string id;

        public DefaultLookupResolver(string id)
        {
            this.id = id;
        }

        public override object Resolve(object originalValue, Dictionary<string, object> originalFields)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new[] { id }));
        }
    }

    public class DefaultTextResolver : ValueResolver
    {
        private readonly string text;

        public DefaultTextResolver(string text)
        {
            this.text = text;
        }

        public override object Resolve(object originalValue, Dictionary<string, object> originalFields)
        {
            return text;
        }
    }

    public class LookupToTextResolver : ValueResolver
    {
        private readonly string[] fieldNames;
        private Dictionary<string, AirtableRecord> tableLookup;
        private string originalFieldName;
        private string lookupFieldName;
        private readonly Dictionary<string, AirtableRecord> table;

        public LookupToTextResolver(Dictionary<string, AirtableRecord> table, string[] fieldNames)
        {
            this.table = table;
            this.fieldNames = fieldNames;
        }

        public LookupToTextResolver(Dictionary<string, AirtableRecord> table, string originalFieldName, string[] fieldNames)
        {
            this.table = table;
            this.fieldNames = fieldNames;
            this.originalFieldName = originalFieldName;
        }

        public LookupToTextResolver(Dictionary<string, AirtableRecord> table, Dictionary<string, AirtableRecord> tableLookup, string originalFieldName, string lookupFieldName, string[] fieldNames)
        {
            this.table = table;
            this.tableLookup = tableLookup;
            this.originalFieldName = originalFieldName;
            this.lookupFieldName = lookupFieldName;
            this.fieldNames = fieldNames;
        }

        public override object Resolve(object originalValue, Dictionary<string, object> originalFields)
        {
            if (tableLookup == null)
            {
                string json = null;

                if (!string.IsNullOrWhiteSpace(originalFieldName) && originalFields.TryGetValue(originalFieldName, out object originalFieldValue))
                {
                    json = "" + originalFieldValue;
                }
                else
                {
                    json = "" + originalValue;
                }

                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                string[] refIDs = JsonConvert.DeserializeObject<string[]>(json);
                if (refIDs != null && refIDs.Length > 0 && table.TryGetValue(refIDs[0], out AirtableRecord record))
                {
                    if (fieldNames.Length > 1)
                    {
                        List<object> values = new List<object>();

                        foreach (string fieldName in fieldNames)
                        {
                            object value = record.GetField(fieldName);
                            if (value != null)
                            {
                                values.Add(value);
                            }
                        }

                        if (values.Count > 0)
                        {
                            return string.Join(" ", values);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return record.GetField(fieldNames[0]);
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (originalFields.TryGetValue(originalFieldName, out object originalFieldValue))
                {
                    string json = "" + originalFieldValue;

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return null;
                    }

                    string[] lookupIDs = JsonConvert.DeserializeObject<string[]>(json);
                    if (lookupIDs != null && lookupIDs.Length > 0 && tableLookup.TryGetValue(lookupIDs[0], out AirtableRecord recordLookup))
                    {
                        json = "" + recordLookup.GetField(lookupFieldName);

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            return null;
                        }

                        string[] recordIDs = JsonConvert.DeserializeObject<string[]>(json);

                        if (recordIDs != null && recordIDs.Length > 0 && table.TryGetValue(recordIDs[0], out AirtableRecord record))
                        {
                            if (fieldNames.Length > 1)
                            {
                                List<object> values = new List<object>();

                                foreach (string fieldName in fieldNames)
                                {
                                    object value = record.GetField(fieldName);
                                    if (value != null)
                                    {
                                        values.Add(value);
                                    }
                                }

                                if (values.Count > 0)
                                {
                                    return string.Join(" ", values);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return record.GetField(fieldNames[0]);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class LookupToLookupResolver : ValueResolver
    {
        private readonly string lookupFieldName;
        private readonly string originalFieldName;
        private readonly Dictionary<string, AirtableRecord> tableDestination;
        private readonly Dictionary<string, string> idsTeamToMain;
        private readonly Dictionary<string, AirtableRecord> tableLookup;
        private readonly bool mainDestination;

        public LookupToLookupResolver(Dictionary<string, AirtableRecord> tableDestination, Dictionary<string, string> idsTeamToMain, bool mainDestination = true)
        {
            this.tableDestination = tableDestination;
            this.idsTeamToMain = idsTeamToMain;
            this.mainDestination = mainDestination;
        }

        public LookupToLookupResolver(Dictionary<string, AirtableRecord> tableDestination, Dictionary<string, string> idsTeamToMain,
            Dictionary<string, AirtableRecord> tableLookup, string originalFieldName, string lookupFieldName, bool mainDestination = true)
        {
            this.tableDestination = tableDestination;
            this.idsTeamToMain = idsTeamToMain;
            this.tableLookup = tableLookup;
            this.originalFieldName = originalFieldName;
            this.lookupFieldName = lookupFieldName;
            this.mainDestination = mainDestination;
        }

        public override object Resolve(object originalValue, Dictionary<string, object> originalFields)
        {
            if (mainDestination)
            {
                if (tableLookup == null)
                {
                    string json = "" + originalValue;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return null;
                    }

                    string[] teamDbIDs = JsonConvert.DeserializeObject<string[]>(json);
                    if (teamDbIDs == null)
                    {
                        return null;
                    }

                    List<string> mainDbIDs = new List<string>();
                    foreach (string teamDbID in teamDbIDs)
                    {
                        if (idsTeamToMain.TryGetValue(teamDbID, out string mainDbID) && tableDestination.ContainsKey(mainDbID))
                        {
                            mainDbIDs.Add(mainDbID);
                        }
                    }

                    if (mainDbIDs.Count > 0)
                    {
                        return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(mainDbIDs.ToArray()));
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (originalFields.TryGetValue(originalFieldName, out object originalFieldValue))
                    {
                        string json = "" + originalFieldValue;

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            return null;
                        }

                        string[] lookupIDs = JsonConvert.DeserializeObject<string[]>(json);
                        if (lookupIDs == null)
                        {
                            return null;
                        }

                        List<string> mainDbIDs = new List<string>();
                        foreach (string lookupID in lookupIDs)
                        {
                            if (tableLookup.TryGetValue(lookupID, out AirtableRecord recordLookup))
                            {
                                json = "" + recordLookup.GetField(lookupFieldName);

                                if (!string.IsNullOrWhiteSpace(json))
                                {
                                    string[] teamDbIDs = JsonConvert.DeserializeObject<string[]>(json);

                                    if (teamDbIDs != null)
                                    {
                                        foreach (string teamDbID in teamDbIDs)
                                        {
                                            if (idsTeamToMain.TryGetValue(teamDbID, out string mainDbID) && tableDestination.ContainsKey(mainDbID))
                                            {
                                                mainDbIDs.Add(mainDbID);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (mainDbIDs.Count > 0)
                        {
                            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(mainDbIDs.ToArray()));
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (tableLookup == null)
                {
                    string json = "" + originalValue;
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return null;
                    }

                    string[] mainDbIDs = JsonConvert.DeserializeObject<string[]>(json);
                    if (mainDbIDs == null)
                    {
                        return null;
                    }

                    List<string> teamDbIDs = new List<string>();
                    foreach (string mainDbID in mainDbIDs)
                    {
                        foreach (KeyValuePair<string, AirtableRecord> pair in tableDestination)
                        {
                            if (idsTeamToMain.TryGetValue(pair.Value.Id, out string mDbID) && mDbID == mainDbID)
                            {
                                teamDbIDs.Add(pair.Value.Id);
                            }
                        }
                    }

                    if (teamDbIDs.Count > 0)
                    {
                        return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(teamDbIDs.ToArray()));
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (originalFields.TryGetValue(originalFieldName, out object originalFieldValue))
                    {
                        string json = "" + originalFieldValue;

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            return null;
                        }

                        string[] lookupIDs = JsonConvert.DeserializeObject<string[]>(json);
                        if (lookupIDs == null)
                        {
                            return null;
                        }

                        List<string> teamDbIDs = new List<string>();
                        foreach (string lookupID in lookupIDs)
                        {
                            if (tableLookup.TryGetValue(lookupID, out AirtableRecord recordLookup))
                            {
                                json = "" + recordLookup.GetField(lookupFieldName);

                                if (!string.IsNullOrWhiteSpace(json))
                                {
                                    string[] mainDbIDs = JsonConvert.DeserializeObject<string[]>(json);

                                    if (mainDbIDs != null)
                                    {
                                        foreach (string mainDbID in mainDbIDs)
                                        {
                                            foreach (KeyValuePair<string, AirtableRecord> pair in tableDestination)
                                            {
                                                if (idsTeamToMain.TryGetValue(pair.Value.Id, out string mDbID) && mDbID == mainDbID)
                                                {
                                                    teamDbIDs.Add(pair.Value.Id);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (teamDbIDs.Count > 0)
                        {
                            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(teamDbIDs.ToArray()));
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }

}
