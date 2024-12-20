namespace FlightManagement.Models
{
  public class SearchRequest
{
    public string From { get; set; } 
    public string To { get; set; } 
    public DateTime? Date { get; set; } 
}

}