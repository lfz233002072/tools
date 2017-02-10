using System;

namespace Lfz.Data.Nh.Conventions {
    
    /// <summary>
    /// This attribute is used to mark relationships which need to be eagerly fetched with the parent object,
    /// thus defining an aggregate in terms of DDD
    /// </summary>
    public class AggregateAttribute : Attribute {
    }
}

