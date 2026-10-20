using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Results;

namespace Itmo.ObjectOrientedProgramming.Lab1.Contracts;

public abstract class Contract {
  public AsteroidBelt Belt { get; }

  public Fleet Fleet { get; }

  public PriceList PriceList { get; }

  protected Contract(AsteroidBelt belt, Fleet fleet, PriceList priceList) {
    ArgumentNullException.ThrowIfNull(belt);
    ArgumentNullException.ThrowIfNull(fleet);
    ArgumentNullException.ThrowIfNull(priceList);
    Belt = belt;
    Fleet = fleet;
    PriceList = priceList;
  }

  /// <summary>
  /// Returns a rejection for contract-specific restrictions, or null when they are satisfied.
  /// </summary>
  public virtual RejectedContract? Validate(decimal flightTime) {
    return null;
  }

  public abstract bool CanStartTrip(decimal elapsedTime, decimal minedVolume, decimal flightTime);

  public abstract bool CanStartCycle(decimal elapsedTime, decimal minedVolume, decimal flightTime);
}
