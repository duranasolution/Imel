namespace ImelMVC.Models
{
    public class AuditLogModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PerformedBy { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
