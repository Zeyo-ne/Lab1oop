namespace Itmo.ObjectOrientedProgramming.Lab1.Ships;

public sealed class Prospect : Ship {
  public Prospect() : base("Prospect", 1000m, 5m, 1500m) {
  }

  public override decimal GetMiningVolume(int cycleNumber) {
    if (cycleNumber == 1) {
      return 75m;
    }

    return 150m;
  }
}
