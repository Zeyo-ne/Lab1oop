namespace Itmo.ObjectOrientedProgramming.Lab1.Ships;

public abstract class Ship {
  public string Name { get; }

  public decimal Capacity { get; }

  public decimal Speed { get; }

  public decimal HourlyRent { get; }

  protected Ship(string name, decimal capacity, decimal speed, decimal hourlyRent) {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(speed);
    ArgumentOutOfRangeException.ThrowIfNegative(hourlyRent);
    Name = name;
    Capacity = capacity;
    Speed = speed;
    HourlyRent = hourlyRent;
  }

  /// <summary>
  /// Returns the volume for a one-based fleet cycle within the current trip.
  /// Implementations must be immutable and return a non-negative volume.
  /// </summary>
  public abstract decimal GetMiningVolume(int cycleNumber);
}
