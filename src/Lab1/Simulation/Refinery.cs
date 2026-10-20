using System.Collections.ObjectModel;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;

namespace Itmo.ObjectOrientedProgramming.Lab1.Simulation;

internal sealed class Refinery {
  private const int _portionSize = 100;
  private readonly Ore _ore;
  private readonly Dictionary<Mineral, long> _minerals;

  public long RemainingOreUnits { get; private set; }

  public IReadOnlyDictionary<Mineral, long> Minerals { get; }

  public Refinery(Ore ore) {
    _ore = ore;
    _minerals = ore.MineralYield.Keys.ToDictionary(mineral => mineral,
                                                   _ => 0L);
    Minerals = new ReadOnlyDictionary<Mineral, long>(_minerals);
  }

  public void Process(decimal tripVolume) {
    // Fractional ore units are discarded separately after each trip.
    long units = checked((long)decimal.Floor(tripVolume / _ore.UnitVolume));
    long storedUnits = checked(RemainingOreUnits + units);
    long portions = storedUnits / _portionSize;
    RemainingOreUnits = storedUnits % _portionSize;
    foreach (KeyValuePair<Mineral, int> yield in _ore.MineralYield) {
      _minerals[yield.Key] = checked(_minerals[yield.Key] + (portions * yield.Value));
    }
  }
}
