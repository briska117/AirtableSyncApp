using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AirTableDatabase.DBModels
{
    [Keyless]
    public class WindowsServiceProjectSynchronization
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string SyncEventId { get; set; }
        public DateTime SyncTime { get; set; }
    }
}
