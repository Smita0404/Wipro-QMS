namespace QMS.Core.Models;

public class OperationResult
{
    public bool Success { get; set; }
    public int? ObjectId { get; set; }
    public string? Message { get; set; }
    public Exception? Exception { get; set; }
    public object? Payload { get; set; }
    public bool Deleted { get; set; }
}
