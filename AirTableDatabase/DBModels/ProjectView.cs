using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class ProjectView
    {
        public Project Project { get; set; }   
        public List<ClientPrefix> ClientPrefixes { get; set; }
        public List<CountryPrefix> CountryPrefixes { get; set; }    
        public List<RelatedTable> RelatedTables { get; set; }  
        
        public List<CollectionMode> CollectionModes { get; set; }   

    }
}
