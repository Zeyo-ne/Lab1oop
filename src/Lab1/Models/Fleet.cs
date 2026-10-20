using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;

namespace Itmo.ObjectOrientedProgramming.Lab1.Models;

public sealed class Fleet {
  public IReadOnlyList<Ship> Ships { get; }

  public IStorageStrategy StorageStrategy { get; }

  public decimal Speed { get; }

  public decimal HourlyRent { get; }

  public Fleet(IEnumerable<Ship> ships, IStorageStrategy storageStrategy) {
    ArgumentNullException.ThrowIfNull(ships);
    ArgumentNullException.ThrowIfNull(storageStrategy);
    Ship[] orderedShips = ships.ToArray();
    if (orderedShips.Length == 0 || orderedShips.Any(ship => ship is null)) {
      throw new ArgumentException("A fleet must contain ships and no null entries.", nameof(ships));
    }

    Ships = Array.AsReadOnly(orderedShips);
    StorageStrategy = storageStrategy;
    Speed = orderedShips.Min(ship => ship.Speed);
    HourlyRent = orderedShips.Sum(ship => ship.HourlyRent);
  }
}
