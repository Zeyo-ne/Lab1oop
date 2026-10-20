namespace Itmo.ObjectOrientedProgramming.Lab1.Storage;

/// <summary>
/// Mutable cargo belonging to a single calculation, never to a ship definition.
/// </summary>
public sealed class CargoHold {
  public decimal Capacity { get; }

  public decimal LoadedVolume { get; private set; }

  public decimal FreeVolume => Capacity - LoadedVolume;

  public CargoHold(decimal capacity) {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
    Capacity = capacity;
  }

  /// <summary>
  /// Loads the entire volume if it fits, otherwise leaves the hold unchanged.
  /// </summary>
  public bool TryLoad(decimal volume) {
    ArgumentOutOfRangeException.ThrowIfNegative(volume);
    if (volume > FreeVolume) {
      return false;
    }

    LoadedVolume += volume;
    return true;
  }
}
