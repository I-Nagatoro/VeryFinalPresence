public class UpdatePresenceRequest
{
    public DateTime Date { get; set; }
    public int LessonNumber { get; set; }
    public int StudentId { get; set; }
    public bool NewTypeAttendance { get; set; }
    public int GroupId { get; set; }
}