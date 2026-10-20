namespace Itmo.ObjectOrientedProgramming.Lab1.Storage;

public sealed class SeparateHoldsStrategy : IStorageStrategy {
  public bool TryPlace(IReadOnlyList<CargoHold> holds, int miningShipIndex, decimal volume) {
    ArgumentNullException.ThrowIfNull(holds);
    ArgumentOutOfRangeException.ThrowIfNegative(miningShipIndex);
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(miningShipIndex, holds.Count);
    return holds[miningShipIndex].TryLoad(volume);
  }
}
