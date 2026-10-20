using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;

namespace Itmo.ObjectOrientedProgramming.Lab1.Models;

public sealed class AsteroidBelt {
  public string Name { get; }

  public decimal Distance { get; }

  public Ore Ore { get; }

  public AsteroidBelt(string name, decimal distance, Ore ore) {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    ArgumentOutOfRangeException.ThrowIfNegative(distance);
    ArgumentNullException.ThrowIfNull(ore);
    Name = name;
    Distance = distance;
    Ore = ore;
  }
}
