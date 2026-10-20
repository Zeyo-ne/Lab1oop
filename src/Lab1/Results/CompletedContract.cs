using System.Collections.ObjectModel;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;

namespace Itmo.ObjectOrientedProgramming.Lab1.Results;

public sealed record CompletedContract : CalculationResult {
  public decimal WorkTime { get; }

  public int TripCount { get; }

  public decimal MinedVolume { get; }

  public IReadOnlyDictionary<Mineral, long> Minerals { get; }

  public long RemainingOreUnits { get; }

  public decimal Revenue { get; }

  public decimal RentalCost { get; }

  public decimal Tax { get; }

  public decimal Profit => Revenue - RentalCost - Tax;

  internal CompletedContract(decimal workTime, int tripCount, decimal minedVolume,
                             IReadOnlyDictionary<Mineral, long> minerals, long remainingOreUnits,
                             decimal revenue, decimal rentalCost, decimal tax) {
    WorkTime = workTime;
    TripCount = tripCount;
    MinedVolume = minedVolume;
    Minerals = new ReadOnlyDictionary<Mineral, long>(new Dictionary<Mineral, long>(minerals));
    RemainingOreUnits = remainingOreUnits;
    Revenue = revenue;
    RentalCost = rentalCost;
    Tax = tax;
  }
}
