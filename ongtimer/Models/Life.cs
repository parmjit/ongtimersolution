namespace ongtimer.Models
{
    public class Life
    {
        public Guid Id { get; set; }
        public int PendingLives { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
