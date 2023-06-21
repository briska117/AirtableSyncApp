using AirTableDatabase.DBModels;
using AirTableWebApi.Services.SyncEvents;
using AirTableWebApi.Services.Projects;
using Navmii.AirtableSync;
using Navmii.AirTableSyncNetcore6;
using System.Text;
using AirTableWebApi.Services.CollectionModes;
using AirTableWebApi.Services.RelatedTables;

namespace AirTableWebApi.Services.AirTableSync
{
    public class AirTableSyncService: IAirTableSyncService
    {
        private readonly IProjectsService projectsService;
        private readonly ISyncEventsService eventsService;
        private readonly ICollectionModeService collectionMode;
        private readonly IRelatedTablesService relatedTablesService;

        public AirTableSyncService(IProjectsService projectsService, ISyncEventsService eventsService, ICollectionModeService collectionMode,
            IRelatedTablesService relatedTablesService)
        {
            this.projectsService = projectsService;
            this.eventsService = eventsService;
            this.collectionMode = collectionMode;
            this.relatedTablesService = relatedTablesService;
        }

        private async void StartAirtableSyncProcces(Project project, string syncEventId, string[] mainList, string[] teamList)
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
                    //if (sync.Execute(out logtread))
                    //{
                    //    history.Comment= Logger.ReadLogFile(); 
                    //    history.StatusSync = StatusSync.Finish;

                    //    await this.eventsService.AddSyncEventHistory(history);

                    //}
                    //else
                    //{
                    //    history.Comment = Logger.ReadLogFile();
                    //    history.StatusSync = StatusSync.Error;
                    //    await this.eventsService.AddSyncEventHistory(history);
                    //}

                    await Task.Delay(10000); history.Comment = Logger.ReadLogFile(); ;
                    history.StatusSync = StatusSync.Finish;
                    await this.eventsService.AddSyncEventHistory(history);
                }
            }
            catch (Exception ex)
            {
                history.Comment = Logger.ReadLogFile(); ;
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
            List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
            string[] main = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToArray();
            string[] team = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToArray();

            this.StartAirtableSyncProcces(project,history.SyncEventHistoryId,main,team);
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
            List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
            string[] main = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToArray();
            string[] team = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToArray();

            this.StartAirtableSyncProcces(project, history.SyncEventHistoryId, main, team);
            return syncEvent;
        }
   
    }
}
