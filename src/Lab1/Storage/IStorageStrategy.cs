namespace Itmo.ObjectOrientedProgramming.Lab1.Storage;

/// <summary>
/// Strategies are stateless. A failed placement must leave every hold unchanged.
/// Successful placement stores the entire volume, never a partial mining yield.
/// </summary>
public interface IStorageStrategy {
  bool TryPlace(IReadOnlyList<CargoHold> holds, int miningShipIndex, decimal volume);
}
