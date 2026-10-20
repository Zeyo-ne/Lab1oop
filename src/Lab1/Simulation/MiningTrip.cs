using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;

namespace Itmo.ObjectOrientedProgramming.Lab1.Simulation;

internal sealed class MiningTrip {
  private readonly Fleet _fleet;
  private readonly IReadOnlyList<CargoHold> _holds;

  private int _cycleNumber = 1;

  public MiningTrip(Fleet fleet) {
    _fleet = fleet;
    _holds = Array.AsReadOnly(fleet.Ships.Select(ship => new CargoHold(ship.Capacity)).ToArray());
  }

  public decimal MineCycle() {
    decimal minedVolume = 0m;
    for (int index = 0; index < _fleet.Ships.Count; index++) {
      decimal volume = _fleet.Ships[index].GetMiningVolume(_cycleNumber);
      if (volume > 0m && _fleet.StorageStrategy.TryPlace(_holds, index, volume)) {
        minedVolume += volume;
      }
    }

    if (minedVolume > 0m) {
      _cycleNumber++;
    }

    return minedVolume;
  }
}
