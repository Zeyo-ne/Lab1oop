namespace Itmo.ObjectOrientedProgramming.Lab1.Results;

public sealed record RejectedContract : CalculationResult {
  public RejectionReason Reason { get; }

  public string Description { get; }

  public RejectedContract(RejectionReason reason, string description) {
    ArgumentException.ThrowIfNullOrWhiteSpace(description);
    Reason = reason;
    Description = description;
  }
}
