using System.Collections.ObjectModel;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;

namespace Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;

public sealed class Ore {
  public string Name { get; }

  public decimal UnitVolume { get; }

  public IReadOnlyDictionary<Mineral, int> MineralYield { get; }

  public Ore(string name, decimal unitVolume, IReadOnlyDictionary<Mineral, int> mineralYield) {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitVolume);
    ArgumentNullException.ThrowIfNull(mineralYield);
    if (mineralYield.Count == 0) {
      throw new ArgumentException("Ore must yield at least one mineral.", nameof(mineralYield));
    }

    foreach (int quantity in mineralYield.Values) {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(mineralYield));
    }

    Name = name;
    UnitVolume = unitVolume;
    MineralYield = new ReadOnlyDictionary<Mineral, int>(new Dictionary<Mineral, int>(mineralYield));
  }
}
