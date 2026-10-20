using System.Collections.ObjectModel;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;

namespace Itmo.ObjectOrientedProgramming.Lab1.Models;

public sealed class PriceList {
  public IReadOnlyDictionary<Mineral, decimal> Prices { get; }

  public PriceList(IReadOnlyDictionary<Mineral, decimal> prices) {
    ArgumentNullException.ThrowIfNull(prices);
    foreach (decimal price in prices.Values) {
      ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(prices));
    }

    Prices = new ReadOnlyDictionary<Mineral, decimal>(new Dictionary<Mineral, decimal>(prices));
  }
}
