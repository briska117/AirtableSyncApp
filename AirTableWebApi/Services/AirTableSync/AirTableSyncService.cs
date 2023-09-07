using AirTableDatabase.DBModels;
using AirTableWebApi.Services.SyncEvents;
using AirTableWebApi.Services.Projects;
using Navmii.AirtableSync;
using Navmii.AirTableSyncNetcore6;
using System.Text;
using AirTableWebApi.Services.CollectionModes;
using AirTableWebApi.Services.RelatedTables;
using AirTableWebApi.Services.AirTableFields;
using AirTableWebApi.Services.AirTableApi;
using Navmii.AirTableSyncNetcore6.AirtablesModels;
using System.Linq;

namespace AirTableWebApi.Services.AirTableSync
{
    public class AirTableSyncService: IAirTableSyncService
    {
        private readonly IProjectsService projectsService;
        private readonly ISyncEventsService eventsService;
        private readonly ICollectionModeService collectionMode;
        private readonly IRelatedTablesService relatedTablesService;
        private readonly IAirTableFieldsService airTableFields;
        private readonly IAirTableApiService airTableApiService;

        public AirTableSyncService(IProjectsService projectsService, 
            ISyncEventsService eventsService, 
            ICollectionModeService collectionMode,
            IRelatedTablesService relatedTablesService,
            IAirTableFieldsService airTableFields,
            IAirTableApiService airTableApiService)
        {
            this.projectsService = projectsService;
            this.eventsService = eventsService;
            this.collectionMode = collectionMode;
            this.relatedTablesService = relatedTablesService;
            this.airTableFields = airTableFields;
            this.airTableApiService = airTableApiService;
        }

        private async Task StartAirtableSyncProcces(Project project, string syncEventId, string[] mainList, string[] teamList)
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
                    TableListCentral = mainList,
                    TableListLocal = teamList
                };
                using (Synchronizer sync = new Synchronizer(settings))
                {
                    StringBuilder logtread = new StringBuilder();

                    history.Comment = $"Airtable Sync Data Base {settings.MainDatabaseID} InProccess {DateTime.UtcNow}";
                    history.StatusSync = StatusSync.InProccess;
                    await this.eventsService.AddSyncEventHistory(history);
                    if (sync.Execute())
                    {
                        history.Comment = $"Complete Sync Data Base {settings.MainDatabaseID} {DateTime.UtcNow}";

                        history.StatusSync = StatusSync.Complete;

                        await this.eventsService.AddSyncEventHistory(history);

                    }
                    else
                    {
                        history.Comment = $"Error Sync Data Base {settings.MainDatabaseID} {DateTime.UtcNow}";
                        history.StatusSync = StatusSync.Error;
                        await this.eventsService.AddSyncEventHistory(history);
                    }


                }
            }
            catch (Exception ex)
            {
                history.Comment = $"{ex.InnerException}";
                history.StatusSync = StatusSync.Error;
                await this.eventsService.AddSyncEventHistory(history);
            } finally{
                await Task.Delay(1000);
                history.Comment = Logger.ReadLogFile();
                history.StatusSync = StatusSync.Finish;
                await this.eventsService.AddSyncEventHistory(history);
            }
        }

        public async Task<SyncEvent> ManualAirtableSync(string projectId) {

            DateTime dateManualSync = DateTime.UtcNow;
            SyncEvent syncEvent = new SyncEvent
            {
                ProjectId = projectId,
                EventName = $"Sync project Manual {dateManualSync.ToShortDateString()}",
                SyncTime = dateManualSync,
                IsAtomatic = false
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
            AirTableApiSettings airTableApiSettings = new AirTableApiSettings { DataBaseId = project.MainDatabaseID, AccessToken = project.ApiKey };
            var airtableInfo = await this.airTableApiService.GetProjectDatabaseSchema(airTableApiSettings);
            string[] main = airtableInfo.CentralTables.Select(c => c.Name).ToArray();
            var firstTeam = airtableInfo.Teams.FirstOrDefault();
            string[] team = firstTeam.TeamTables.Select(c => c.Name).ToArray();
            var mainFilter = SyncTablesConfig.RigsModeMain;
            var teamFilter = SyncTablesConfig.RigsModeTeam;
            main = main.Where(t => mainFilter.Contains(t)).ToArray();
            team = team.Where(t => teamFilter.Contains(t)).ToArray();


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
            AirTableApiSettings airTableApiSettings = new AirTableApiSettings { DataBaseId = project.MainDatabaseID, AccessToken = project.ApiKey };
            var airtableInfo = await this.airTableApiService.GetProjectDatabaseSchema(airTableApiSettings);
            string[] main = airtableInfo.CentralTables.Select(c => c.Name).ToArray();
            var firstTeam = airtableInfo.Teams.FirstOrDefault();
            string[] team = firstTeam.TeamTables.Select(c => c.Name).ToArray();
            var mainFilter = SyncTablesConfig.RigsModeMain;
            var teamFilter = SyncTablesConfig.RigsModeTeam;
            main = main.Where(t => mainFilter.Contains(t)).ToArray();
            team = team.Where(t => teamFilter.Contains(t)).ToArray();

            this.StartAirtableSyncProcces(project, history.SyncEventHistoryId, main, team);
            return syncEvent;
        }
   
    }
}
