using Realtime.Storage.Models;

[StorageKey("score", "heroID", "kills")]
public class Score
{
    public string heroID { get; set; }
    public int kills { get; set; }
}