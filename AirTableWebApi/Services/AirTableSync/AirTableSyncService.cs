using AirTableDatabase.DBModels;
using AirTableWebApi.Services.SyncEvents;
using AirTableWebApi.Services.Projects;
using Navmii.AirtableSync;
using Navmii.AirTableSyncNetcore6;
using System.Text;
using AirTableWebApi.Services.CollectionModes;
using AirTableWebApi.Services.RelatedTables;
using AirTableWebApi.Services.AirTableFields;

namespace AirTableWebApi.Services.AirTableSync
{
    public class AirTableSyncService: IAirTableSyncService
    {
        private readonly IProjectsService projectsService;
        private readonly ISyncEventsService eventsService;
        private readonly ICollectionModeService collectionMode;
        private readonly IRelatedTablesService relatedTablesService;
        private readonly IAirTableFieldsService airTableFields;

        public AirTableSyncService(IProjectsService projectsService, 
            ISyncEventsService eventsService, 
            ICollectionModeService collectionMode,
            IRelatedTablesService relatedTablesService,
            IAirTableFieldsService airTableFields)
        {
            this.projectsService = projectsService;
            this.eventsService = eventsService;
            this.collectionMode = collectionMode;
            this.relatedTablesService = relatedTablesService;
            this.airTableFields = airTableFields;
        }

        private async Task StartAirtableSyncProcces(Project project, string syncEventId, string[] mainList, string[] teamList, List<RelatedTable> tables)
        {

            var history = await this.eventsService.GetSyncEventHistory(syncEventId);
            try
            {
                string ProjectPrefix = $"{project.ClientPrefix.Name} - {project.CollectionMode.Name} - {project.CountryPrefix.Name},{project.ClientPrefix.Name} > {project.CollectionMode.Name} > {project.CountryPrefix.Name}";
                SyncSettings settings = new SyncSettings
                {
                    ApiKey = project.ApiKey,
                    TSheetsApiToken = project.TableSheetsToken,
                    ManualSync = true,
                    SyncTSheets = true,
                    ProjectPrefix = ProjectPrefix,
                    MainDatabaseID = project.MainDatabaseID,
                    BackupPath = "Backup",
                    TableListMain = mainList,
                    TableListTeam = teamList 
                };
                using (Synchronizer sync = new Synchronizer(settings))
                {
                    StringBuilder logtread = new StringBuilder();

                    history.Comment = $"Airtable Sync InProccess {DateTime.UtcNow}";
                    history.StatusSync = StatusSync.InProccess;
                    await this.eventsService.AddSyncEventHistory(history);
                    if (sync.Execute())
                    {
                        history.Comment = Logger.ReadLogFile();
                        history.StatusSync = StatusSync.Finish;

                        await this.eventsService.AddSyncEventHistory(history);

                    }
                    else
                    {
                        history.Comment = Logger.ReadLogFile();
                        history.StatusSync = StatusSync.Error;
                        await this.eventsService.AddSyncEventHistory(history);
                    }

                    await Task.Delay(1000);
                    history.Comment = $"Automatic Sync Complete {DateTime.Now}"; ;
                    history.StatusSync = StatusSync.Finish;
                    await this.eventsService.AddSyncEventHistory(history);
                }
            }
            catch (Exception ex)
            {
                history.Comment = Logger.ReadLogFile(); 
                history.Comment=history.Comment+ $"\n{ex.Message}\n {ex.InnerException}";
                history.StatusSync = StatusSync.Error;
                await this.eventsService.AddSyncEventHistory(history);
            }
        }

        public async Task<SyncEvent> ManualAirtableSync(string projectId) {

            DateTime dateManualSync = DateTime.UtcNow;
            SyncEvent syncEvent = new SyncEvent
            {
                ProjectId = projectId,
                EventName = $"Sync project Manual {dateManualSync.ToShortDateString()}",
                SyncTime = dateManualSync
            };

            await this.eventsService.AddAsyncEvent(syncEvent);

            SyncEventHistory history = new SyncEventHistory
            {
                SyncEventId = syncEvent.SyncEventId,
                Comment = "Start Maual Airtable sync",
                StatusSync = StatusSync.Start,
                StartSync = dateManualSync
            };
            await this.eventsService.AddSyncEventHistory(history);
            Project project = await this.projectsService.GetProjectExtend(projectId);
            List<CollectionModeRelatedTable> collectionModeRelatedTables = await this.collectionMode.GetCollectionModeRelatedTable(project.Mode);
            List<string> relatdTablesIds = collectionModeRelatedTables.Select(c=>c.RelatedTableId).ToList();    
            List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
            relatedTables = relatedTables.Where(r=> relatdTablesIds.Contains(r.RelatedTableId)).ToList();
            string[] main = relatedTables.Where(c => c.MainList).Select(c => c.Name).ToArray();
            string[] team = relatedTables.Where(c => c.TeamList).Select(c => c.Name).ToArray();


            this.StartAirtableSyncProcces(project,history.SyncEventHistoryId,main,team,relatedTables);
            return syncEvent;
        }

        public async Task<SyncEvent> AutomaticAirtableSync(string projectId, string eventId)
        {

            DateTime dateManualSync = DateTime.UtcNow;
            SyncEvent syncEvent =await this.eventsService.GetSyncEvent(eventId);


            SyncEventHistory history = new SyncEventHistory
            {
                SyncEventId = syncEvent.SyncEventId,
                Comment = "Start Automatic Airtable sync",
                StatusSync = StatusSync.Start,
                StartSync = dateManualSync
            };
            await this.eventsService.AddSyncEventHistory(history);
            Project project = await this.projectsService.GetProjectExtend(projectId);
            List<CollectionModeRelatedTable> collectionModeRelatedTables = await this.collectionMode.GetCollectionModeRelatedTable(project.Mode);
            List<string> relatdTablesIds = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToList();
            List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
            relatedTables = relatedTables.Where(r => relatdTablesIds.Contains(r.RelatedTableId)).ToList();
            string[] main = relatedTables.Where(c => c.MainList).Select(c => c.Name).ToArray();
            string[] team = relatedTables.Where(c => c.TeamList).Select(c => c.Name).ToArray();

            this.StartAirtableSyncProcces(project, history.SyncEventHistoryId, main, team,relatedTables);
            return syncEvent;
        }
   
    }
}
