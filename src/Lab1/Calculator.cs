using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Simulation;

namespace Itmo.ObjectOrientedProgramming.Lab1;

public sealed class Calculator {
  private readonly Station _station;

  public Calculator(Station station) {
    ArgumentNullException.ThrowIfNull(station);
    _station = station;
  }

  public CalculationResult Calculate(Contract contract) {
    ArgumentNullException.ThrowIfNull(contract);
    RejectedContract? rejection = ValidateMining(contract);
    if (rejection is not null) {
      return rejection;
    }

    decimal flightTime = contract.Belt.Distance / contract.Fleet.Speed;
    rejection = contract.Validate(flightTime);
    if (rejection is not null) {
      return rejection;
    }

    // Cargo and warehouse contents belong to one calculation, not to the domain objects.
    var refinery = new Refinery(contract.Belt.Ore);
    decimal elapsedTime = 0m;
    decimal minedVolume = 0m;
    int tripCount = 0;
    while (contract.CanStartTrip(elapsedTime, minedVolume, flightTime)) {
      var trip = new MiningTrip(contract.Fleet);
      decimal tripVolume = 0m;
      elapsedTime += flightTime;
      while (contract.CanStartCycle(elapsedTime, minedVolume, flightTime)) {
        decimal cycleVolume = trip.MineCycle();
        if (cycleVolume == 0m) {
          break;
        }

        tripVolume += cycleVolume;
        minedVolume += cycleVolume;
        elapsedTime += 1m;
      }

      elapsedTime += flightTime;
      tripCount++;
      refinery.Process(tripVolume);
    }

    decimal revenue =
        refinery.Minerals.Sum(entry => entry.Value * contract.PriceList.Prices[entry.Key]);
    return new CompletedContract(
        elapsedTime, tripCount, minedVolume, refinery.Minerals, refinery.RemainingOreUnits, revenue,
        contract.Fleet.HourlyRent * elapsedTime, _station.TaxRate * revenue);
  }

  private static RejectedContract? ValidateMining(Contract contract) {
    foreach (Mineral mineral in contract.Belt.Ore.MineralYield.Keys) {
      if (!contract.PriceList.Prices.ContainsKey(mineral)) {
        return new RejectedContract(RejectionReason.MissingMineralPrice,
                                    $"The price list does not contain a price for {mineral.Name}.");
      }
    }

    if (new MiningTrip(contract.Fleet).MineCycle() == 0m) {
      return new RejectedContract(
          RejectionReason.NoMiningCapacity,
          "No ship can mine a complete first-cycle yield with empty holds.");
    }

    return null;
  }
}
