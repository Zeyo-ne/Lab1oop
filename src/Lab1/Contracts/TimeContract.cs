using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Results;

namespace Itmo.ObjectOrientedProgramming.Lab1.Contracts;

public sealed class TimeContract : Contract {
  public decimal Duration { get; }

  public TimeContract(AsteroidBelt belt, Fleet fleet, PriceList priceList, decimal duration)
      : base(belt, fleet, priceList) {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(duration);
    Duration = duration;
  }

  public override RejectedContract? Validate(decimal flightTime) {
    if ((2m * flightTime) + 1m > Duration) {
      return new RejectedContract(
          RejectionReason.InsufficientTime,
          "The contract duration is too short for a round trip and one mining cycle.");
    }

    return null;
  }

  public override bool CanStartTrip(decimal elapsedTime, decimal minedVolume, decimal flightTime) {
    // The task requires a strict time limit when starting a trip.
    return elapsedTime + (2m * flightTime) + 1m < Duration;
  }

  public override bool CanStartCycle(decimal elapsedTime, decimal minedVolume, decimal flightTime) {
    return elapsedTime + 1m + flightTime <= Duration;
  }
}
