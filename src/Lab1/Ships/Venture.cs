namespace Itmo.ObjectOrientedProgramming.Lab1.Ships;

public sealed class Venture : Ship {
  public Venture() : base("Venture", 400m, 4m, 1000m) {
  }

  public override decimal GetMiningVolume(int cycleNumber) {
    return 100m;
  }
}
