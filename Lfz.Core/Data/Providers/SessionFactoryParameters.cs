using System.Collections.Generic;

namespace PMSoft.Data.Providers {
    /// <summary>
    /// 
    /// </summary>
    public class SessionFactoryParameters : DataServiceParameters {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<RecordBlueprint> RecordDescriptors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool CreateDatabase { get; set; }
    }
}
