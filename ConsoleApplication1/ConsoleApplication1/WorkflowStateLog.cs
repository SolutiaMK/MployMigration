//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkflowStateLog
    {
        public int Id { get; set; }
        public System.Guid GlobalEntityId { get; set; }
        public int WorkflowStateId { get; set; }
        public Nullable<int> WorkflowStateSubId { get; set; }
        public Nullable<int> WorkflowStateSubReasonCodeId { get; set; }
        public System.DateTime TimestampBegin { get; set; }
        public Nullable<System.DateTime> TimestampEnd { get; set; }
        public Nullable<int> CreatedById { get; set; }
        public string CreatedByName { get; set; }
    
        public virtual GlobalEntity GlobalEntity { get; set; }
    }
}
