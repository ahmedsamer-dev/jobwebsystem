namespace JobApplicationWeb.Models
{
    public class JobApplication

    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public User User { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
