
using AirtableApiClient;
using Navmii.AirTableSyncNetcore6;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirtableSync
{
    public class Synchronizer : IDisposable
    {
        public const int CurrentVersion = 8;
        private const string Table_TSheets = "TSheets";
        public const string Field_TSheetsUserID = "ID (leave blank if adding a new user)";

        private readonly SyncSettings settings;
        private AirtableBase mainBase = null;
        private AirtableBase teamBase = null;
        private string backupZipPath = null;
        private Dictionary<string, Dictionary<string, AirtableRecord>> dataMain = null;
        private Dictionary<string, Dictionary<string, AirtableRecord>> dataTeam = null;
        private Dictionary<string, DateTime> modifiedMain = null;
        private Dictionary<string, DateTime> modifiedTeam = null;
        private DateTime emptyTime = new DateTime(2000, 1, 1);

        private Dictionary<string, AirtableRecord> teamDatabases = new Dictionary<string, AirtableRecord>();

        private string[] GetTableListMain(UtilityMode mode)
        {
            if (mode == UtilityMode.Rigs)
            {
                return SyncTablesConfig.RigsModeMain;
            }
            else if (mode == UtilityMode.RigsTechM)
            {
                return SyncTablesConfig.RigsModeTechMMain;
            }
            else //if (mode == UtilityMode.Pedestrians)
            {
                return SyncTablesConfig.PedestriansModeMain;
            }
        }

        private string[] GetTableListTeam(UtilityMode mode)
        {
            if (mode == UtilityMode.Rigs)
            {
                return SyncTablesConfig.RigsModeTeam;
            }
            else if (mode == UtilityMode.RigsTechM)
            {
                return SyncTablesConfig.RigsModeTechMTeam;
            }
            else //if (mode == UtilityMode.Pedestrians)
            {
                return SyncTablesConfig.PedestriansModeTeam;
            }
        }

        public Synchronizer(SyncSettings syncSettings)
        {
            settings = syncSettings;
        }

        public bool Execute()
        {
            // Connect to the central database

            mainBase = Connect(settings.MainDatabaseID);

            int version = ReadVersion(out UtilityMode mode);
            if (mode == UtilityMode.None)
            {
                Logger.Write("SYNC TOOL MODE NOT RECOGNIZED");
                return false;
            }
            else if (version != CurrentVersion)
            {
                Logger.Write($"SYNC TOOL AND DB VERSIONS DIFFER:\r\nSync tool version - {CurrentVersion}\r\nDatabase version - {version}");
                return false;
            }

            Logger.Write("######## START ########");

            // Create an empty ZIP archive for database backups
            backupZipPath = CreateBackupZipArchive();

            DateTime start = DateTime.UtcNow;
            // Read data from all the tables in the central DB
            dataMain = DbTools.ReadAllData(mainBase, settings.TableListCentral);
            Logger.Write("Cached Central Database data in {0:#,##0} sec", (DateTime.UtcNow - start).TotalSeconds);

            // Write all tables data into backup ZIP archive 
            //BackupData(string.Format("Central Database ({0})", mainDatabaseID), dataMain);

            if (settings.SyncTSheets)
            {
                // Sync TSheets users into central DB
                SyncTSheets();
            }

            // Read previous last modification time for each table in the central DB
            modifiedMain = ReadModifiedTimes(settings.MainDatabaseID);

            // Filter records in Teams tables referring to local databases
            InitTeamDatabases();

            Dictionary<string, Dictionary<string, int>> perDiemData = new Dictionary<string, Dictionary<string, int>>();
            foreach (KeyValuePair<string, AirtableRecord> pair in teamDatabases)
            {
                // Synchronize each local database
                SyncTeam(pair.Key, pair.Value, mode, perDiemData);
            }

            //if (mode == UtilityMode.Pedestrians)
            //{
            SyncPerDiemToMain(SyncTablesConfig.TSheets, perDiemData);
            //}

            // Read current last modification time for each table in the central DB
            modifiedMain = ReadModified(mainBase, settings.TableListCentral);

            // Save current last modification time for each table in the central DB
            WriteModifiedTimes(settings.MainDatabaseID, modifiedMain);

            Logger.Write("######## END ########");

            return true;
        }

        private int ReadVersion(out UtilityMode mode)
        {
            mode = UtilityMode.None;
            string filter = string.Format("{{DatabaseID}} = '{0}'", settings.MainDatabaseID);
            AirtableRecord record = DbTools.FilterSingleRecord(mainBase, "Sync", filter);

            if (record != null)
            {
                try
                {
                    int v = Convert.ToInt32(record.GetField("Version"));
                    mode = (UtilityMode)(v / 100);
                    return (v % 100);
                }
                catch { }
            }

            return 0;
        }

        private Dictionary<string, DateTime> ReadModifiedTimes(string databaseID)
        {
            string filter = string.Format("{{DatabaseID}} = '{0}'", databaseID);
            AirtableRecord record = DbTools.FilterSingleRecord(mainBase, "Sync", filter);

            if (record != null)
            {
                try
                {
                    Dictionary<string, DateTime> value = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>("" + record.GetField("JSON"));

                    if (value != null)
                    {
                        return value;
                    }
                }
                catch { }
            }

            return new Dictionary<string, DateTime>();
        }

        private void WriteModifiedTimes(string databaseID, Dictionary<string, DateTime> times)
        {
            string json = JsonConvert.SerializeObject(times);

            string filter = string.Format("{{DatabaseID}} = '{0}'", databaseID);
            AirtableRecord record = DbTools.FilterSingleRecord(mainBase, "Sync", filter);

            Dictionary<string, object> fields = new Dictionary<string, object>
            {
                { "DatabaseID", databaseID },
                { "JSON", json }
            };

            if (record == null)
            {
                ExecuteSingleCreate(mainBase.CreateRecord("Sync", new Fields() { FieldsCollection = fields }));
            }
            else
            {
                ExecuteSingleUpdate(mainBase.UpdateRecord("Sync", new Fields() { FieldsCollection = fields }, record.Id));
            }
        }

        private void BackupData(string name, Dictionary<string, Dictionary<string, AirtableRecord>> data)
        {
            using (FileStream zipFileStream = new FileStream(backupZipPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (ZipArchive zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Update))
            {
                foreach (KeyValuePair<string, Dictionary<string, AirtableRecord>> pair in data)
                {
                    string entryName = string.Format("{0}/{1}.json", name, pair.Key);

                    ZipArchiveEntry entry = zipArchive.CreateEntry(entryName);
                    using (StreamWriter writer = new StreamWriter(entry.Open()))
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(pair.Value));
                    }
                }
            }
        }

        private void DeleteOldBackups(string backupPath)
        {
            List<string> delete = new List<string>();

            foreach (string filePath in Directory.EnumerateFiles(backupPath, "*.zip", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string dateName = fileName.Substring(0, "yyyyMMdd".Length);

                if (DateTime.TryParseExact(dateName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) && date.AddDays(7) < DateTime.UtcNow)
                {
                    delete.Add(filePath);
                }
            }

            foreach (string filePath in delete)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { }
            }
        }

        private string CreateBackupZipArchive()
        {
            int number = 0;
            string dateName = string.Format("{0:yyyyMMdd}", DateTime.UtcNow);
            string backupPath = settings.BackupPath;

            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }

            DeleteOldBackups(backupPath);

            string filePath = Path.Combine(backupPath, string.Format("{0}.zip", dateName));
            while (File.Exists(filePath))
            {
                number++;
                filePath = Path.Combine(backupPath, string.Format("{0}_{1}.zip", dateName, number));
            }

            using (FileStream zipFileStream = new FileStream(filePath, FileMode.Create))
            using (ZipArchive zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
            {
            }

            return filePath;
        }

        private Dictionary<string, DateTime> ReadModified(AirtableBase airtableBase, string[] tableNames)
        {
            Dictionary<string, DateTime> modified = new Dictionary<string, DateTime>();

            foreach (string tableName in tableNames)
            {
                DateTime m = DbTools.ReadModified(airtableBase, tableName);
                modified.Add(tableName, m);
            }

            return modified;
        }

        private bool UserDataChanged(Dictionary<string, string> userData, AirtableRecord record)
        {
            string[] fieldNames = new[]
            {
                "First Name (required)",
                "Last Name (required)",
                "Group",
                "cf_Job Title",
                "cf_Work Email",
                "cf_Work Number",
            };

            foreach (string fieldName in fieldNames)
            {
                if (!userData.TryGetValue(fieldName, out string value1))
                {
                    value1 = null;
                }

                value1 = "" + value1;
                string value2 = "" + record.GetField(fieldName);

                if (!value1.Equals(value2))
                {
                    return true;
                }
            }

            return false;
        }

        private void SyncTSheets()
        {
            DateTime start = DateTime.UtcNow;
            TSheetsFetch sheetsFetch = new TSheetsFetch(settings);
            Dictionary<int, Dictionary<string, string>> users = sheetsFetch.FetchUsers();
            Logger.Write("Fetched {0} users from TSheets in {1:#,##0} sec", users.Count, (DateTime.UtcNow - start).TotalSeconds);

            Logger.Write("TSheets to central...");

            List<string> archive = new List<string>();
            Dictionary<int, List<string>> userIDs = new Dictionary<int, List<string>>();

            Dictionary<string, AirtableRecord> tableTSheets = dataMain[SyncTablesConfig.TSheets];

            foreach (KeyValuePair<string, AirtableRecord> pair in tableTSheets)
            {
                string strUserID = "" + pair.Value.GetField(Field_TSheetsUserID);
                if (int.TryParse(strUserID, out int userID))
                {
                    if (users.ContainsKey(userID))
                    {
                        if (!userIDs.TryGetValue(userID, out List<string> recordIDs))
                        {
                            recordIDs = new List<string>();
                            userIDs.Add(userID, recordIDs);
                        }

                        recordIDs.Add(pair.Key);
                    }
                    else if (!DbTools.GetArchived(pair.Value))
                    {
                        archive.Add(pair.Key);
                    }
                }
            }

            int archivedCount = 0;
            int createdCount = 0;
            int updatedCount = 0;

            List<List<IdFields>> multiArchive = new List<List<IdFields>>();
            List<List<IdFields>> multiUpdate = new List<List<IdFields>>();
            List<List<Fields>> multiCreate = new List<List<Fields>>();

            archivedCount += DbTools.PopulateMultiArchiveList(archive, multiArchive);

            foreach (KeyValuePair<int, Dictionary<string, string>> pair in users)
            {
                Dictionary<string, object> fields = new Dictionary<string, object>();
                foreach (KeyValuePair<string, string> p in pair.Value)
                {
                    fields.Add(p.Key, p.Value);
                }

                if (userIDs.TryGetValue(pair.Key, out List<string> recordIDs))
                {
                    foreach (string recordID in recordIDs)
                    {
                        if (tableTSheets.TryGetValue(recordID, out AirtableRecord record) && UserDataChanged(pair.Value, record))
                        {
                            if (multiUpdate.Count == 0 || multiUpdate.Last().Count == 10)
                            {
                                multiUpdate.Add(new List<IdFields>()
                                {
                                    new IdFields(recordID)
                                    {
                                        FieldsCollection = fields
                                    }
                                });
                            }
                            else
                            {
                                multiUpdate.Last().Add(new IdFields(recordID)
                                {
                                    FieldsCollection = fields
                                });
                            }

                            updatedCount++;
                        }
                    }
                }
                else
                {
                    if (multiCreate.Count == 0 || multiCreate.Last().Count == 10)
                    {
                        multiCreate.Add(new List<Fields>()
                        {
                            new Fields()
                            {
                                FieldsCollection = fields
                            }
                        });
                    }
                    else
                    {
                        multiCreate.Last().Add(new Fields()
                        {
                            FieldsCollection = fields
                        });
                    }

                    createdCount++;
                }
            }

            if (createdCount > 0 || updatedCount > 0 || archivedCount > 0)
            {
                if (multiArchive.Count > 0)
                {
                    DbTools.ExecuteMultiUpdate(multiArchive, mainBase, Table_TSheets, dataMain);
                }

                if (multiUpdate.Count > 0)
                {
                    DbTools.ExecuteMultiUpdate(multiUpdate, mainBase, Table_TSheets, dataMain);
                }

                if (multiCreate.Count > 0)
                {
                    DbTools.ExecuteMultiCreate(multiCreate, mainBase, Table_TSheets, dataMain);
                }

                Logger.Write("Added: {0}, updated: {1}, archived: {2}", createdCount, updatedCount, archivedCount);
            }
            else
            {
                Logger.Write("Nothing to sync");
            }
        }

        private string GetDC(AirtableRecord teamRecord)
        {
            string json = "" + teamRecord.GetField("DC");

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            string[] recordIDs = JsonConvert.DeserializeObject<string[]>(json);

            if (recordIDs != null && recordIDs.Length > 0 && dataMain[SyncTablesConfig.TSheets].TryGetValue(recordIDs[0], out AirtableRecord record))
            {
                return "" + record.GetField("Last Name (required)");
            }
            else
            {
                return null;
            }
        }

        private void SyncTeam(string teamDatabaseID, AirtableRecord teamRecord, UtilityMode mode, Dictionary<string, Dictionary<string, int>> perDiemData)
        {
            string teamID = teamRecord.Id;
            string teamName = "" + teamRecord.GetField("Name");
            string dc = GetDC(teamRecord);

            Logger.Write(@"'{0}' (database ID: {1})
---------------------------------
SYNCHRONIZATION START", teamName, teamDatabaseID);

            // Read previous last modification time for each table in local DB
            modifiedTeam = ReadModifiedTimes(teamDatabaseID);

            // Connect to local DB
            using (teamBase = Connect(teamDatabaseID))
            {
                DateTime start = DateTime.UtcNow;
                // Read data from all the tables in local DB
                dataTeam = DbTools.ReadAllData(teamBase, settings.TableListLocal);
                Logger.Write("Cached data in {0:#,##0} sec", (DateTime.UtcNow - start).TotalSeconds);

                // Write all tables data into backup ZIP archive 
                //BackupData(string.Format("{0} ({1})", teamName, teamDatabaseID), dataTeam);


                // TABLE "TSheets"
                SyncTableToTeam(
                    SyncTablesConfig.TSheets,
                    SyncTablesConfig.TSheetsFields,
                    new string[0],
                    true,
                    new TSheetsRecordFilter(teamName));

                //if (mode == UtilityMode.Pedestrians)
                //{
                UpdatePerDiemData(SyncTablesConfig.TSheets, perDiemData);
                //}


                // TABLE "Camps"
                if (mode == UtilityMode.Rigs || mode == UtilityMode.RigsTechM)
                {
                    SyncTableToTeam(
                        SyncTablesConfig.Camps,
                        SyncTablesConfig.CampsFields,
                        new string[0],
                        true,
                        new TeamRecordFilter(teamID, "Team"),
                        null,
                        new Dictionary<string, ValueResolver>
                        {
                            { "Camp Lead", new LookupToLookupResolver(dataTeam[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam), false) }
                        });
                }
                else if (mode == UtilityMode.Pedestrians)
                {
                    SyncTableToTeam(
                        SyncTablesConfig.Camps,
                       SyncTablesConfig.CampsFieldsPedestrians,
                        new string[0],
                        true,
                        new TeamRecordFilter(teamID, "Team"),
                        null,
                        new Dictionary<string, ValueResolver>
                        {
                            { "Camp Lead", new LookupToLookupResolver(dataTeam[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam), false) }
                        });
                }


                // TABLE "Systems"/"Packs"
                if (mode == UtilityMode.Rigs || mode == UtilityMode.RigsTechM)
                {
                    SyncTableToTeam(
                    SyncTablesConfig.Systems,
                    SyncTablesConfig.SystemsFields,
                    new string[0],
                    true,
                    new TeamRecordFilter(teamID, "Owning Team"));
                }
                else if (mode == UtilityMode.Pedestrians)
                {
                    SyncTableToTeam(
                    SyncTablesConfig.Packs,
                    SyncTablesConfig.PacksFields,
                    new string[0],
                    true,
                    new TeamRecordFilter(teamID, "Owning Team"));
                }


                if (mode == UtilityMode.Rigs || mode == UtilityMode.RigsTechM)
                {
                    // TABLE "Cars"
                    SyncTableToTeam(
                        SyncTablesConfig.Cars,
                       SyncTablesConfig.CarsFieldsTeam,
                        new[]
                        {
                            "Photos Of Damage",
                            "Maintenance Docs"
                        },
                        false,
                        null,
                        new TeamRecordFilter(teamID, "Owning Team"));

                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.Cars,
                        null,
                        SyncTablesConfig.CarsFieldsMain,
                        new[]
                        {
                                "Photos Of Damage",
                                "Maintenance Docs"
                        },
                        false,
                        new Dictionary<string, ValueResolver>
                        {
                                { "Owning Team", new DefaultLookupResolver(teamID) }
                        });


                    // TABLE "Rigs"
                    SyncTableToTeam(
                    SyncTablesConfig.Rigs,
                    SyncTablesConfig.RigsFields,
                    new string[0],
                    true,
                    new TeamRecordFilter(teamID, "Owning Team"),
                    null,
                    new Dictionary<string, ValueResolver>
                    {
                        { "Car", new LookupToLookupResolver(dataTeam[SyncTablesConfig.Cars], DbTools.GetMainDbIDs(SyncTablesConfig.Cars, dataTeam), false) },
                        { "System", new LookupToLookupResolver(dataTeam[SyncTablesConfig.Systems], DbTools.GetMainDbIDs(SyncTablesConfig.Systems, dataTeam), false) }
                    });
                }


                // TABLE "Chase Vehicles"
                SyncTableToMain(
                    teamName,
                    SyncTablesConfig.ChaseVehicles,
                    null,
                   SyncTablesConfig.ChaseVehiclesFields,
                    new[]
                    {
                        "Rental Agreement",
                        "Return Reciept"
                    },
                    true,
                    new Dictionary<string, ValueResolver>
                    {
                        { "Owning Team", new DefaultLookupResolver(teamID) }
                    });


                if (mode == UtilityMode.Rigs || mode == UtilityMode.RigsTechM)
                {
                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.DailyReportDetails,
                        new[] { "Project" },
                        SyncTablesConfig.DailyReportDetailsFields,
                        new[]
                        {
                            "Warboard"
                        },
                        true,
                        new Dictionary<string, ValueResolver>
                        {
                            { "Camp", new LookupToLookupResolver(dataMain[SyncTablesConfig.Camps], DbTools.GetMainDbIDs(SyncTablesConfig.Camps, dataTeam)) },
                            { "DC", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) },
                            { "CCs", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) },
                        });

                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.Transponders,
                        null,
                        SyncTablesConfig.TranspondersFields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>
                        {
                            { "Vehicle #", new LookupToLookupResolver(dataMain[SyncTablesConfig.Cars], DbTools.GetMainDbIDs(SyncTablesConfig.Cars, dataTeam)) }
                        });

                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.RigAssignments,
                        new[] { "Project" },
                        SyncTablesConfig.RigAssignmentFields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>()
                        {
                            //{ "Assignment Date", new LookupToTextResolver(dataTeam["Daily Report Details"], new[] { "Report Date" }) },
                            { "Assignment Date", new LookupToLookupResolver(dataMain[SyncTablesConfig.DailyReportDetails], DbTools.GetMainDbIDs(SyncTablesConfig.DailyReportDetails, dataTeam)) },
                            { "Rig", new LookupToLookupResolver(dataMain[SyncTablesConfig.Rigs], DbTools.GetMainDbIDs(SyncTablesConfig.Rigs, dataTeam)) },
                            { "Camp", new LookupToLookupResolver(dataMain[SyncTablesConfig.Camps], DbTools.GetMainDbIDs(SyncTablesConfig.Camps, dataTeam)) },
                            { "Team", new DefaultLookupResolver(teamID) },
                            { "Driver", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) },
                            { "Operator", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) }
                        });

                    SyncTableToMain(
                        teamName,
                       SyncTablesConfig.ReDriveList,
                        new[] { "Re-Drive Date" },
                        SyncTablesConfig.ReDriveListFields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>()
                        {
                            { "Teams", new DefaultLookupResolver(teamID) },
                            { "System", new LookupToLookupResolver(dataMain[SyncTablesConfig.Systems], DbTools.GetMainDbIDs(SyncTablesConfig.Systems, dataTeam), true) }
                        });

                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.WeeklyStatusUpdate,
                        new[] { "Estimated End of Collection Date" },
                        SyncTablesConfig.WeeklyStatusUpdateFields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>()
                        {
                            { "Team", new DefaultLookupResolver(teamID) },
                            { "Current Camp", new LookupToLookupResolver(dataMain[SyncTablesConfig.Camps], DbTools.GetMainDbIDs(SyncTablesConfig.Camps, dataTeam), true) }
                        });

                    if (mode == UtilityMode.RigsTechM)
                    {
                        SyncTableToMain(
                            teamName,
                            SyncTablesConfig.MaintenanceSpendDetail,
                            new[] { "Vehicle Number" },
                           SyncTablesConfig.MaintenanceSpendDetailFields,
                            new[] { "Maintenance Documents" },
                            true,
                            new Dictionary<string, ValueResolver>()
                            {
                                { "Team", new DefaultLookupResolver(teamID) },
                                { "Vehicle Number", new LookupToLookupResolver(dataMain[SyncTablesConfig.Cars], DbTools.GetMainDbIDs(SyncTablesConfig.Cars, dataTeam), true) }
                            });

                        SyncTableToMain(
                            teamName,
                            SyncTablesConfig.MiscSpendDetail,
                            new[] { "Vehicle Number" },
                           SyncTablesConfig.MiscSpendDetailFields,
                            new[] { "Receipt" },
                            true,
                            new Dictionary<string, ValueResolver>()
                            {
                                { "Team", new DefaultLookupResolver(teamID) },
                                { "Vehicle Number", new LookupToLookupResolver(dataMain[SyncTablesConfig.Rigs], DbTools.GetMainDbIDs(SyncTablesConfig.Rigs, dataTeam), true) }
                            });
                    }
                }
                else if (mode == UtilityMode.Pedestrians)
                {
                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.DailyCampReports,
                        new[] { "Report Date", "Camp" },
                       SyncTablesConfig.DailyCampReportsfields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>
                        {
                            { "Camp", new LookupToLookupResolver(dataMain[SyncTablesConfig.Camps], DbTools.GetMainDbIDs(SyncTablesConfig.Camps, dataTeam)) },
                            { "FC", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) },
                            { "T3(s)", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) },
                            { "Team", new DefaultLookupResolver(teamID) },
                        });

                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.PackAssignments,
                        new[] { "Assignment Date", "Pack", "Camp", "Status" },
                       SyncTablesConfig.PackAssignmentsFields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>()
                        {
                            { "Assignment Date", new LookupToLookupResolver(dataMain[SyncTablesConfig.DailyCampReports], DbTools.GetMainDbIDs(SyncTablesConfig.DailyCampReports, dataTeam)) },
                            { "Pack", new LookupToLookupResolver(dataMain[SyncTablesConfig.Packs], DbTools.GetMainDbIDs(SyncTablesConfig.Packs, dataTeam)) },
                            { "Spotter", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) },
                            { "Operator", new LookupToLookupResolver(dataMain[SyncTablesConfig.TSheets], DbTools.GetMainDbIDs(SyncTablesConfig.TSheets, dataTeam)) },
                            { "Team", new DefaultLookupResolver(teamID) }
                        });

                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.Interactions,
                        new[] { "Date And Time Of Interaction", "Interaction Type" },
                        SyncTablesConfig.InteractionsFields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>()
                        {
                            { "Pack Assignment", new LookupToLookupResolver(dataMain[SyncTablesConfig.PackAssignments], DbTools.GetMainDbIDs(SyncTablesConfig.PackAssignments, dataTeam)) },
                            { "Team", new DefaultLookupResolver(teamID) }
                        });

                    SyncTableToMain(
                        teamName,
                        SyncTablesConfig.CommuteTimes,
                        new[] { "Date and Commute Start Time", "Pack and Operators" },
                       SyncTablesConfig.CommuteTimesFields,
                        new string[0],
                        true,
                        new Dictionary<string, ValueResolver>()
                        {
                            { "Pack and Operators", new LookupToLookupResolver(dataMain[SyncTablesConfig.PackAssignments], DbTools.GetMainDbIDs(SyncTablesConfig.PackAssignments, dataTeam)) },
                            { "Team", new DefaultLookupResolver(teamID) }
                        });
                }

                // clear team data cache
                foreach (KeyValuePair<string, Dictionary<string, AirtableRecord>> pair in dataTeam)
                {
                    pair.Value.Clear();
                }
                dataTeam.Clear();

                // Read current last modification time for each table in local DB
                modifiedTeam = ReadModified(teamBase, settings.TableListLocal);

                // Save current last modification time for each table in local DB
                WriteModifiedTimes(teamDatabaseID, modifiedTeam);

                Logger.Write(@"SYNCHRONIZATION END
---------------------------------");
            }
        }


        private Dictionary<string, object> SyncFields(Dictionary<string, object> originalFields, string[] fieldNames, Dictionary<string, ValueResolver> referenceResolvers, bool forceNulls)
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();

            foreach (string fieldName in fieldNames)
            {
                if (!originalFields.TryGetValue(fieldName, out object fieldValue))
                {
                    fieldValue = null;
                }

                if (referenceResolvers != null && referenceResolvers.TryGetValue(fieldName, out ValueResolver referenceResolver))
                {
                    fieldValue = referenceResolver.Resolve(fieldValue, originalFields);
                }

                if (forceNulls || fieldValue != null)
                {
                    fields.Add(fieldName, fieldValue);
                }
            }

            return fields;
        }

        private DateTime GetLastModifiedMain(string tableName)
        {
            if (modifiedMain.TryGetValue(tableName, out DateTime value))
            {
                return value;
            }
            else
            {
                return emptyTime;
            }
        }

        private DateTime GetLastModifiedTeam(string tableName)
        {
            if (modifiedTeam.TryGetValue(tableName, out DateTime value))
            {
                return value;
            }
            else
            {
                return emptyTime;
            }
        }

        private void SyncAttachments(AirtableRecord originalRecord, Dictionary<string, object> fields, string[] attachmentFields, bool forceNulls)
        {
            foreach (string attachmentField in attachmentFields)
            {
                List<string> urls = new List<string>();

                IEnumerable<AirtableAttachment> attachments = originalRecord.GetAttachmentField(attachmentField);
                if (attachments != null)
                {
                    foreach (AirtableAttachment attachment in attachments)
                    {
                        urls.Add(string.Format("{{ \"url\": \"{0}\" }}", attachment.Url));
                    }
                }

                if (urls.Count > 0)
                {
                    string json = string.Format("[ {0} ]", string.Join(", ", urls));
                    fields.Add(attachmentField, JsonConvert.DeserializeObject(json));
                }
                else if (forceNulls)
                {
                    fields.Add(attachmentField, JsonConvert.DeserializeObject("[]"));
                }
            }
        }

        private bool FieldsPopulated(string[] fields, AirtableRecord record)
        {
            if (fields != null && fields.Length > 0)
            {
                foreach (string field in fields)
                {
                    object value = record.GetField(field);

                    if (value == null || string.IsNullOrWhiteSpace("" + value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void UpdatePerDiemData(string tableName, Dictionary<string, Dictionary<string, int>> perDiemData)
        {
            Dictionary<string, string> mainDbIDs = DbTools.GetMainDbIDs(tableName, dataTeam);
            Dictionary<string, AirtableRecord> tableTeam = dataTeam[tableName];

            foreach (KeyValuePair<string, AirtableRecord> pair in tableTeam)
            {
                string idTeam = pair.Key;
                AirtableRecord recordTeam = pair.Value;

                if (mainDbIDs.TryGetValue(idTeam, out string idMain))
                {
                    Dictionary<string, int> fields = DbTools.GetPerDiemFields(recordTeam.Fields);

                    if (!perDiemData.TryGetValue(idMain, out Dictionary<string, int> data))
                    {
                        data = new Dictionary<string, int>();
                        perDiemData.Add(idMain, data);
                    }

                    foreach (KeyValuePair<string, int> f in fields)
                    {
                        if (data.ContainsKey(f.Key))
                        {
                            data[f.Key] += f.Value;
                        }
                        else
                        {
                            data.Add(f.Key, f.Value);
                        }
                    }
                }
            }
        }

        private bool SyncPerDiemFields(AirtableRecord recordMain, Dictionary<string, int> perDiemTeam, out Dictionary<string, object> fields)
        {
            fields = new Dictionary<string, object>();

            bool modified = false;

            Dictionary<string, int> perDiemMain = DbTools.GetPerDiemFields(recordMain.Fields);

            foreach (KeyValuePair<string, int> pair in perDiemMain)
            {
                if (!perDiemTeam.TryGetValue(pair.Key, out int value))
                {
                    fields.Add(pair.Key, null);
                    modified = true;
                }
                else if (value != pair.Value)
                {
                    fields.Add(pair.Key, value);
                    modified = true;
                }
            }

            foreach (KeyValuePair<string, int> pair in perDiemTeam)
            {
                if (!perDiemMain.ContainsKey(pair.Key))
                {
                    fields.Add(pair.Key, pair.Value);
                    modified = true;
                }
            }

            return modified;
        }

        private bool SyncPerDiemToMain(string tableName, Dictionary<string, Dictionary<string, int>> perDiemData)
        {
            Logger.Write("'{0}': per diem data to central...", tableName);

            Dictionary<string, AirtableRecord> tableMain = dataMain[tableName];

            List<List<IdFields>> multiUpdate = new List<List<IdFields>>();

            int updatedCount = 0;
            foreach (KeyValuePair<string, Dictionary<string, int>> pair in perDiemData)
            {
                if (tableMain.TryGetValue(pair.Key, out AirtableRecord recordMain))
                {
                    if (SyncPerDiemFields(recordMain, pair.Value, out Dictionary<string, object> fields))
                    {
                        if (multiUpdate.Count == 0 || multiUpdate.Last().Count == 10)
                        {
                            multiUpdate.Add(new List<IdFields>()
                            {
                                new IdFields(pair.Key)
                                {
                                    FieldsCollection = fields
                                }
                            });
                        }
                        else
                        {
                            multiUpdate.Last().Add(new IdFields(pair.Key)
                            {
                                FieldsCollection = fields
                            });
                        }

                        updatedCount++;
                    }
                }
            }

            if (updatedCount > 0)
            {
                if (multiUpdate.Count > 0)
                {
                    DbTools.ExecuteMultiUpdate(multiUpdate, mainBase, tableName, dataMain);
                }

                Logger.Write($"Updated: {updatedCount}");

                return true;
            }
            else
            {
                Logger.Write("Nothing to sync");

                return false;
            }
        }

        private string ReadStringField(AirtableRecord record, string fieldName)
        {
            if (record.Fields.TryGetValue(fieldName, out object value) && value != null)
            {
                return value.ToString();
            }
            else
            {
                return null;
            }
        }

        private bool SyncTableToMain(string teamName, string tableName, string[] mandatoryFields, string[] fieldNames, string[] attachmentFields, bool ignoreCentralChanges, Dictionary<string, ValueResolver> referenceResolvers = null)
        {
            Logger.Write("'{0}': from local to central...", tableName);

            Dictionary<string, AirtableRecord> tableMain = dataMain[tableName];
            Dictionary<string, AirtableRecord> tableTeam = dataTeam[tableName];

            DateTime lastModifiedMain = GetLastModifiedMain(tableName);
            DateTime lastModifiedTeam = GetLastModifiedTeam(tableName);

            List<AirtableRecord> create = new List<AirtableRecord>();
            Dictionary<string, AirtableRecord> update = new Dictionary<string, AirtableRecord>();
            Dictionary<string, string> mainDbIDs = DbTools.GetMainDbIDs(tableName, dataTeam);

            foreach (KeyValuePair<string, AirtableRecord> pair in tableTeam)
            {
                string idTeam = pair.Key;
                AirtableRecord recordTeam = pair.Value;

                if (FieldsPopulated(mandatoryFields, recordTeam) && !DbTools.GetArchived(recordTeam))
                {
                    if (!mainDbIDs.TryGetValue(idTeam, out string idMain)
                        || !tableMain.TryGetValue(idMain, out AirtableRecord recordMain))
                    {
                        create.Add(recordTeam);
                    }
                    else if (DbTools.GetModified(recordTeam) > lastModifiedTeam
                        || (ignoreCentralChanges && DbTools.GetModified(recordMain) > lastModifiedMain))
                    {
                        if (!update.TryGetValue(idMain, out AirtableRecord recordDuplicate)
                            || DbTools.GetModified(recordTeam) > DbTools.GetModified(recordDuplicate))
                        {
                            update[idMain] = recordTeam;
                        }
                    }
                }
            }

            if (update.Count > 0 || create.Count > 0)
            {
                List<List<IdFields>> multiUpdate = new List<List<IdFields>>();

                foreach (KeyValuePair<string, AirtableRecord> pair in update)
                {
                    Dictionary<string, object> fields = SyncFields(pair.Value.Fields, fieldNames, referenceResolvers, !ignoreCentralChanges);
                    SyncAttachments(pair.Value, fields, attachmentFields, !ignoreCentralChanges);

                    if (multiUpdate.Count == 0 || multiUpdate.Last().Count == 10)
                    {
                        multiUpdate.Add(new List<IdFields>()
                            {
                                new IdFields(pair.Key)
                                {
                                    FieldsCollection = fields
                                }
                            });
                    }
                    else
                    {
                        multiUpdate.Last().Add(new IdFields(pair.Key)
                        {
                            FieldsCollection = fields
                        });
                    }
                }

                DbTools.ExecuteMultiUpdate(multiUpdate, mainBase, tableName, dataMain);

                List<List<(Fields, string)>> multiCreate = new List<List<(Fields, string)>>();

                foreach (AirtableRecord record in create)
                {
                    Dictionary<string, object> fields = SyncFields(record.Fields, fieldNames, referenceResolvers, !ignoreCentralChanges);
                    SyncAttachments(record, fields, attachmentFields, !ignoreCentralChanges);

                    if (multiCreate.Count == 0 || multiCreate.Last().Count == 10)
                    {
                        multiCreate.Add(new List<(Fields, string)>()
                        {
                            (new Fields()
                            {
                                FieldsCollection = fields
                            },
                            record.Id)
                        });
                    }
                    else
                    {
                        multiCreate.Last().Add(
                            (new Fields()
                            {
                                FieldsCollection = fields
                            },
                            record.Id)
                        );
                    }
                }

                DbTools.ExecuteMultiCreate(multiCreate, tableName, mainBase, teamBase, dataMain, dataTeam);

                Logger.Write($"Added: {create.Count}, updated: {update.Count}");

                return true;
            }
            else
            {
                Logger.Write("Nothing to sync");

                return false;
            }
        }

        private bool SyncTableToTeam(string tableName, string[] fieldNames, string[] attachmentFields, bool ignoreLocalChanges, RecordFilter filterFetch = null, RecordFilter filterActive = null, Dictionary<string, ValueResolver> referenceResolvers = null)
        {
            Logger.Write("'{0}': from central to local...", tableName);

            DateTime lastModifiedMain = GetLastModifiedMain(tableName);
            DateTime lastModifiedTeam = GetLastModifiedTeam(tableName);

            Dictionary<string, AirtableRecord> tableMain = new Dictionary<string, AirtableRecord>();
            HashSet<string> activeIDs = new HashSet<string>();

            // filter main DB records and active records
            foreach (KeyValuePair<string, AirtableRecord> pair in dataMain[tableName])
            {
                string idMain = pair.Key;
                AirtableRecord recordMain = pair.Value;

                if (filterFetch == null || filterFetch.Check(recordMain))
                {
                    tableMain.Add(idMain, recordMain);

                    if (!DbTools.GetArchived(recordMain) && (filterActive == null || filterActive.Check(recordMain)))
                    {
                        activeIDs.Add(idMain);
                    }
                }
            }

            Dictionary<string, AirtableRecord> tableTeam = dataTeam[tableName];

            Dictionary<string, string> mainDbIDs = DbTools.GetMainDbIDs(tableName, dataTeam);

            Dictionary<string, string> update = new Dictionary<string, string>();
            HashSet<string> unchanged = new HashSet<string>();

            List<string> archive = new List<string>();
            foreach (KeyValuePair<string, AirtableRecord> pair in tableTeam)
            {
                string idTeam = pair.Key;
                AirtableRecord recordTeam = pair.Value;

                if (!mainDbIDs.TryGetValue(idTeam, out string idMain) || !tableMain.TryGetValue(idMain, out AirtableRecord recordMain))
                {
                    if (!DbTools.GetArchived(recordTeam))
                    {
                        archive.Add(idTeam);
                    }
                }
                else if (DbTools.GetModified(recordMain) > lastModifiedMain
                    || (ignoreLocalChanges && DbTools.GetModified(recordTeam) > lastModifiedTeam)
                    || (!activeIDs.Contains(idMain) && !DbTools.GetArchived(recordTeam)))
                {
                    update.Add(idMain, idTeam);
                }
                else
                {
                    unchanged.Add(idMain);
                }
            }

            int archivedCount = 0;
            int createdCount = 0;
            int updatedCount = 0;

            List<List<IdFields>> multiArchive = new List<List<IdFields>>();

            archivedCount += DbTools.PopulateMultiArchiveList(archive, multiArchive);

            List<List<IdFields>> multiUpdate = new List<List<IdFields>>();
            List<List<(Fields, string)>> multiCreate = new List<List<(Fields, string)>>();

            foreach (KeyValuePair<string, AirtableRecord> pair in tableMain)
            {
                string idMain = pair.Key;
                AirtableRecord recordMain = pair.Value;

                Dictionary<string, object> fields = SyncFields(recordMain.Fields, fieldNames, referenceResolvers, true);
                SyncAttachments(recordMain, fields, attachmentFields, true);

                if (update.TryGetValue(idMain, out string idTeam))
                {
                    if (!activeIDs.Contains(idMain))
                    {
                        fields[DbTools.Field_Archived] = true;
                    }

                    if (multiUpdate.Count == 0 || multiUpdate.Last().Count == 10)
                    {
                        multiUpdate.Add(new List<IdFields>()
                        {
                            new IdFields(idTeam)
                            {
                                FieldsCollection = fields
                            }
                        });
                    }
                    else
                    {
                        multiUpdate.Last().Add(new IdFields(idTeam)
                        {
                            FieldsCollection = fields
                        });
                    }

                    updatedCount++;
                }
                else if (!unchanged.Contains(idMain))
                {
                    if (!activeIDs.Contains(idMain))
                    {
                        fields[DbTools.Field_Archived] = true;
                    }

                    if (multiCreate.Count == 0 || multiCreate.Last().Count == 10)
                    {
                        multiCreate.Add(new List<(Fields, string)>()
                        {
                            (new Fields()
                            {
                                FieldsCollection = fields
                            }, idMain)
                        });
                    }
                    else
                    {
                        multiCreate.Last().Add((new Fields()
                        {
                            FieldsCollection = fields
                        }, idMain));
                    }

                    createdCount++;
                }
            }

            if (archivedCount > 0 || updatedCount > 0 || createdCount > 0)
            {
                if (multiArchive.Count > 0)
                {
                    DbTools.ExecuteMultiUpdate(multiArchive, teamBase, tableName, dataTeam);
                }

                if (multiUpdate.Count > 0)
                {
                    DbTools.ExecuteMultiUpdate(multiUpdate, teamBase, tableName, dataTeam);
                }

                if (multiCreate.Count > 0)
                {
                    DbTools.ExecuteMultiCreate(multiCreate, teamBase, tableName, dataTeam);
                }

                Logger.Write("Added: {0}, updated: {1}, archived: {2}", createdCount, updatedCount, archivedCount);

                return true;
            }
            else
            {
                Logger.Write("Nothing to sync");

                return false;
            }
        }

        private void ExecuteSingleUpdate(Task<AirtableCreateUpdateReplaceRecordResponse> task)
        {
            task.Wait();

            AirtableCreateUpdateReplaceRecordResponse response = task.Result;

            if (!response.Success)
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

        private string ExecuteSingleCreate(Task<AirtableCreateUpdateReplaceRecordResponse> task)
        {
            task.Wait();

            AirtableCreateUpdateReplaceRecordResponse response = task.Result;

            if (!response.Success)
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
            else
            {
                return response.Record.Id;
            }
        }

        private AirtableRecord[] ExecuteMultiTask(Task<AirtableCreateUpdateReplaceMultipleRecordsResponse> task)
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

        private void InitTeamDatabases()
        {
            List<AirtableRecord> teams = DbTools.ReadTable(mainBase, "Teams");

            foreach (AirtableRecord team in teams)
            {
                string teamDatabaseID = "" + team.GetField("DatabaseID");

                if (!string.IsNullOrWhiteSpace(teamDatabaseID) && !teamDatabases.ContainsKey(teamDatabaseID))
                {
                    teamDatabases.Add(teamDatabaseID, team);
                }
            }
        }

        private AirtableBase Connect(string databaseID)
        {
            return new AirtableBase(settings.ApiKey, databaseID);
        }

        public void Dispose()
        {
            if (mainBase != null)
            {
                mainBase.Dispose();
            }

            teamDatabases.Clear();

            if (dataMain != null)
            {
                foreach (KeyValuePair<string, Dictionary<string, AirtableRecord>> pair in dataMain)
                {
                    pair.Value.Clear();
                }
                dataMain.Clear();
            }
        }
    }
}
