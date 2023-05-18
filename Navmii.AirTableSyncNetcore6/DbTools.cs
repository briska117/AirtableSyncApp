using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;

namespace Navmii.AirtableSync
{
    public class DbTools
    {
        public const string Field_Archived = "Archived";

        private const string Field_Modified = "Modified";
        private const string Field_Table = "Table";
        private const string Field_Central = "Central";
        private const string Field_Local = "Local";
        private const string Field_PerDiemDays = "Per Diem Days";

        private const string Table_Sync_IDs = "Sync IDs";

        public static DateTime emptyTime = new DateTime(2000, 1, 1);

        public static int PopulateMultiArchiveList(List<string> archive, List<List<IdFields>> multiArchive)
        {
            int count = 0;

            foreach (string id in archive)
            {
                Dictionary<string, object> fields = new Dictionary<string, object> { { Field_Archived, true } };

                if (multiArchive.Count == 0 || multiArchive.Last().Count == 10)
                {
                    multiArchive.Add(new List<IdFields>()
                        {
                            new IdFields(id)
                            {
                                FieldsCollection = fields
                            }
                        });
                }
                else
                {
                    multiArchive.Last().Add(new IdFields(id)
                    {
                        FieldsCollection = fields
                    });
                }

                count++;
            }

            return count;
        }

        public static void ExecuteMultiCreate(List<List<(Fields, string)>> creates, AirtableBase database, string tableName, Dictionary<string, Dictionary<string, AirtableRecord>> dataCache)
        {
            Dictionary<string, AirtableRecord> tableCache = dataCache[tableName];
            Dictionary<string, AirtableRecord> idsCache = dataCache[Table_Sync_IDs];

            foreach (List<(Fields, string)> c in creates)
            {
                List<Fields> f = c.ConvertAll<Fields>(x => x.Item1);
                List<string> m = c.ConvertAll<string>(x => x.Item2);

                AirtableRecord[] records = ExecuteMultiTask(database.CreateMultipleRecords(tableName, f.ToArray()));
                UpdateTableCache(records, tableCache);

                List<string> t = records.ToList().ConvertAll<string>(x => x.Id);
                List<Fields> ids = new List<Fields>();
                for (int i = 0; i < t.Count; i++)
                {
                    ids.Add(new Fields 
                    {
                        FieldsCollection = new Dictionary<string, object> 
                        {
                            { Field_Table, tableName},
                            { Field_Central, m[i]},
                            { Field_Local, t[i]}
                        }
                    });
                }

                records = ExecuteMultiTask(database.CreateMultipleRecords(Table_Sync_IDs, ids.ToArray()));
                UpdateTableCache(records, idsCache);
            }
        }
        public static void ExecuteMultiCreate(List<List<(Fields, string)>> creates, string tableName, AirtableBase dbMain, AirtableBase dbTean, 
            Dictionary<string, Dictionary<string, AirtableRecord>> cacheMain, Dictionary<string, Dictionary<string, AirtableRecord>> cacheTeam)
        {
            Dictionary<string, AirtableRecord> tableCache = cacheMain[tableName];
            Dictionary<string, AirtableRecord> idsCache = cacheTeam[Table_Sync_IDs];

            foreach (List<(Fields, string)> c in creates)
            {
                List<Fields> f = c.ConvertAll<Fields>(x => x.Item1);
                List<string> t = c.ConvertAll<string>(x => x.Item2);

                AirtableRecord[] records = ExecuteMultiTask(dbMain.CreateMultipleRecords(tableName, f.ToArray()));
                UpdateTableCache(records, tableCache);

                List<string> m = records.ToList().ConvertAll<string>(x => x.Id);
                List<Fields> ids = new List<Fields>();
                for (int i = 0; i < m.Count; i++)
                {
                    ids.Add(new Fields
                    {
                        FieldsCollection = new Dictionary<string, object>
                        {
                            { Field_Table, tableName},
                            { Field_Central, m[i]},
                            { Field_Local, t[i]}
                        }
                    });
                }

                records = ExecuteMultiTask(dbTean.CreateMultipleRecords(Table_Sync_IDs, ids.ToArray()));
                UpdateTableCache(records, idsCache);
            }
        }

        public static void ExecuteMultiCreate(List<List<Fields>> creates, AirtableBase database, string tableName, Dictionary<string, Dictionary<string, AirtableRecord>> dataCache)
        {
            Dictionary<string, AirtableRecord> tableCache = dataCache[tableName];

            foreach (List<Fields> create in creates)
            {
                AirtableRecord[] records = ExecuteMultiTask(database.CreateMultipleRecords(tableName, create.ToArray()));
                UpdateTableCache(records, tableCache);
            }
        }

        public static void ExecuteMultiUpdate(List<List<IdFields>> updates, AirtableBase database, string tableName, Dictionary<string, Dictionary<string, AirtableRecord>> dataCache)
        {
            Dictionary<string, AirtableRecord> tableCache = dataCache[tableName];

            foreach (List<IdFields> update in updates)
            {
                AirtableRecord[] records = ExecuteMultiTask(database.UpdateMultipleRecords(tableName, update.ToArray()));
                UpdateTableCache(records, tableCache);
            }
        }

        public static Dictionary<string, Dictionary<string, AirtableRecord>> ReadAllData(AirtableBase airtableBase, string[] tableNames)
        {
            
            Dictionary<string, Dictionary<string, AirtableRecord>> tables = new Dictionary<string, Dictionary<string, AirtableRecord>>();

            foreach (string tableName in tableNames)
            {
                List<AirtableRecord> records = ReadTable(airtableBase, tableName);
                Dictionary<string, AirtableRecord> table = TableToDictionary(records);
                tables.Add(tableName, table);
            }

            return tables;
        }

        private static Dictionary<string, AirtableRecord> TableToDictionary(List<AirtableRecord> records)
        {
            Dictionary<string, AirtableRecord> dict = new Dictionary<string, AirtableRecord>();

            foreach (AirtableRecord record in records)
            {
                dict.Add(record.Id, record);
            }

            return dict;
        }

        private static AirtableRecord[] ExecuteMultiTask(Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task)
        {
            task.Wait();

            AirtableCreateUpdateReplaceMultipleRecordsResponse response = task.Result;

            if (response.Success)
            {
                return response.Records;
            }
            else
            {
                if (response.AirtableApiError is AirtableApiException)
                {
                    if (response.AirtableApiError is AirtableInvalidRequestException)
                    {
                        AirtableInvalidRequestException error = (AirtableInvalidRequestException)response.AirtableApiError;
                        Logger.Write("ERROR DETAILS: {0}", error.DetailedErrorMessage);
                    }

                    throw response.AirtableApiError;
                }
                else
                {
                    throw new System.Data.DataException("Unknown error");
                }
            }
        }

        public static bool GetArchived(AirtableRecord record)
        {
            var archived = record.GetField(Field_Archived);
            if (archived == null)
            {
                return false;
            }
            bool myBool = bool.Parse(archived.ToString());

            return myBool;
        }

        private static string GetString(AirtableRecord record, string fieldName)
        {
            object value = record.GetField(fieldName);

            if (value != null && value is string)
            {
                return (string)value;
            }
            else
            {
                return null;
            }
        }

        public static DateTime GetModified(AirtableRecord record)
        {
            object d = record.GetField(Field_Modified);

            if (d is DateTime)
            {
                return (DateTime)d;
            }
            else
            {
                return emptyTime;
            }
        }

        public static DateTime ReadModified(AirtableBase airtableBase, string tableName)
        {
            DateTime modified = emptyTime;

            string offset = null;

            do
            {
                Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(tableName, offset, new[] { Field_Modified });
                task.Wait();
                AirtableListRecordsResponse response = task.Result;

                if (response.Success)
                {
                    foreach (AirtableRecord record in response.Records)
                    {
                        DateTime m = GetModified(record);
                        if (m > modified)
                        {
                            modified = m;
                        }
                    }

                    offset = response.Offset;
                }
                else if (response.AirtableApiError is AirtableApiException)
                {
                    if (response.AirtableApiError is AirtableInvalidRequestException)
                    {
                        AirtableInvalidRequestException error = (AirtableInvalidRequestException)response.AirtableApiError;
                        Logger.Write("ERROR DETAILS: {0}", error.DetailedErrorMessage);
                    }

                    throw response.AirtableApiError;
                }
                else
                {
                    throw new System.Data.DataException("Unknown error");
                }
            }
            while (offset != null);

            return modified;
        }

        public static AirtableRecord FilterSingleRecord(AirtableBase airtableBase, string tableName, string filter)
        {
            Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(tableName, null, null, filter);
            task.Wait();
            AirtableListRecordsResponse response = task.Result;
            if (response.Success && response.Records.Count() > 0)
            {
                return response.Records.First();
            }
            else
            {
                return null;
            }
        }

        public static void UpdateTableCache(AirtableRecord[] records, Dictionary<string, AirtableRecord> table)
        {
            foreach (AirtableRecord record in records)
            {
                table[record.Id] = record;
            }
        }

        public static Dictionary<string, string> GetMainDbIDs(string tableName, Dictionary<string, Dictionary<string, AirtableRecord>> dataCache)
        {
            Dictionary<string, AirtableRecord> tableIDs = dataCache[Table_Sync_IDs];

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (KeyValuePair<string, AirtableRecord> pair in tableIDs)
            {
                string tName = GetString(pair.Value, Field_Table);

                if (tName == tableName)
                {
                    string centralID = GetString(pair.Value, Field_Central);
                    string localID = GetString(pair.Value, Field_Local);

                    if (!result.ContainsKey(localID))
                    {
                        result.Add(localID, centralID);
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, int> GetPerDiemFields(Dictionary<string, object> originalFields)
        {
            Dictionary<string, int> fields = new Dictionary<string, int>();

            foreach (KeyValuePair<string, object> pair in originalFields)
            {
                if (pair.Key.EndsWith(Field_PerDiemDays) && pair.Value != null)
                {
                    fields.Add(pair.Key, Convert.ToInt32(pair.Value));
                }
            }

            return fields;
        }

        public static List<AirtableRecord> ReadTable(AirtableBase airtableBase, string tableName)
        {
            List<AirtableRecord> records = new List<AirtableRecord>();

            string offset = null;

            do
            {
                Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(tableName, offset);
                task.Wait();
                AirtableListRecordsResponse response = task.Result;

                if (response.Success)
                {
                    records.AddRange(response.Records.ToList());
                    offset = response.Offset;
                }
                else if (response.AirtableApiError is AirtableApiException)
                {
                    if (response.AirtableApiError is AirtableInvalidRequestException)
                    {
                        AirtableInvalidRequestException error = (AirtableInvalidRequestException)response.AirtableApiError;
                        Logger.Write("ERROR DETAILS: {0}", error.DetailedErrorMessage);
                    }

                    throw response.AirtableApiError;
                }
                else
                {
                    throw new System.Data.DataException("Unknown error");
                }
            }
            while (offset != null);

            return records;
        }
    }
}
