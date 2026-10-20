using Itmo.ObjectOrientedProgramming.Lab1.Models;

namespace Itmo.ObjectOrientedProgramming.Lab1.Contracts;

public sealed class VolumeContract : Contract {
  public decimal TargetVolume { get; }

  public VolumeContract(AsteroidBelt belt, Fleet fleet, PriceList priceList, decimal targetVolume)
      : base(belt, fleet, priceList) {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetVolume);
    TargetVolume = targetVolume;
  }

  public override bool CanStartTrip(decimal elapsedTime, decimal minedVolume, decimal flightTime) {
    return minedVolume < TargetVolume;
  }

  public override bool CanStartCycle(decimal elapsedTime, decimal minedVolume, decimal flightTime) {
    return minedVolume < TargetVolume;
  }
}
