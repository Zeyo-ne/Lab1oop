namespace Itmo.ObjectOrientedProgramming.Lab1.Storage;

public sealed class SharedHoldStrategy : IStorageStrategy {
  public bool TryPlace(IReadOnlyList<CargoHold> holds, int miningShipIndex, decimal volume) {
    ArgumentNullException.ThrowIfNull(holds);
    ArgumentOutOfRangeException.ThrowIfNegative(miningShipIndex);
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(miningShipIndex, holds.Count);
    ArgumentOutOfRangeException.ThrowIfNegative(volume);
    if (holds.Sum(hold => hold.FreeVolume) < volume) {
      return false;
    }

    decimal remaining = volume - LoadAvailable(holds[miningShipIndex], volume);
    for (int index = 0; index < holds.Count && remaining > 0m; index++) {
      if (index == miningShipIndex) {
        continue;
      }

      remaining -= LoadAvailable(holds[index], remaining);
    }

    return true;
  }

  private static decimal LoadAvailable(CargoHold hold, decimal volume) {
    decimal loadedVolume = Math.Min(hold.FreeVolume, volume);
    hold.TryLoad(loadedVolume);
    return loadedVolume;
  }
}
