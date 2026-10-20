namespace Itmo.ObjectOrientedProgramming.Lab1.Models;

public sealed class Station {
  public decimal TaxRate { get; }

  public Station(decimal taxRate) {
    ArgumentOutOfRangeException.ThrowIfLessThan(taxRate, 0m);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(taxRate, 1m);
    TaxRate = taxRate;
  }
}
