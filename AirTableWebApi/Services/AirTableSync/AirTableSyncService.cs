using AirTableDatabase.DBModels;
using AirTableWebApi.Services.AsyncEvents;
using AirTableWebApi.Services.Projects;
using Navmii.AirtableSync;
using Navmii.AirTableSyncNetcore6;

namespace AirTableWebApi.Services.AirTableSync
{
    public class AirTableSyncService: IAirTableSyncService
    {
        private readonly IProjectsService projectsService;
        private readonly IAsyncEventsService eventsService;

        public AirTableSyncService(IProjectsService projectsService, IAsyncEventsService eventsService)
        {
            this.projectsService = projectsService;
            this.eventsService = eventsService;
        }

        private async void StartAirtableSyncProcces(Project project, string syncEventId)
        {

            //var history = await this.eventsService.GetSyncEventHistory(syncEventId);
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
                    BackupPath = "Backup"
                };
                using (Synchronizer sync = new Synchronizer(settings))
                {
                    //history.Comment = $"Airtable Sync InProccess {DateTime.UtcNow}";
                    //history.StatusSync = StatusSync.InProccess;
                    //await this.eventsService.UpdateSyncEventHistory(history);
                    if (sync.Execute())
                    {
                        //history.Comment= $"Airtable Sync Success {DateTime.UtcNow}";
                        //history.StatusSync = StatusSync.Finish;

                    }
                    else
                    {
                        //history.Comment = $"Airtable Sync error {DateTime.UtcNow}";
                        //history.StatusSync = StatusSync.Error;
                    }
                }
            }
            catch (Exception ex)
            {
                //history.Comment = $"Airtable Sync error: {ex.Message}"; 
                //history.StatusSync = StatusSync.Error;
            }
            finally
            {
                //.FinishAsync = DateTime.Now;
                //await this.eventsService.UpdateSyncEventHistory(history);
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
                StatusAsync = StatusSync.Start,
                StartAsync = dateManualSync
            };
            await this.eventsService.AddSyncEventHistory(history);
            Project project = await this.projectsService.GetProjectExtend(projectId);

            this.StartAirtableSyncProcces(project,history.SyncEventHistoryId);
            return syncEvent;
        }
    }
}
