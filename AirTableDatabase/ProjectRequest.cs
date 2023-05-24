using AirTableDatabase.DBModels;

namespace AirTableDatabase
{
    public class ProjectRequest
    {
        public ProjectForm Project { get; set; }
        public List<SyncEvent> Events { get; set; }
    }
}
