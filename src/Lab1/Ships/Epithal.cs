namespace Itmo.ObjectOrientedProgramming.Lab1.Ships;

public sealed class Epithal : Ship {
  public Epithal() : base("Epithal", 3000m, 2m, 500m) {
  }

  public override decimal GetMiningVolume(int cycleNumber) {
    return 0m;
  }
}
