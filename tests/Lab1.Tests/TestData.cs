using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

internal static class TestData {
  public static AsteroidBelt CreateBelt(Ore? ore = null) {
    return new AsteroidBelt("Пояс A", 4m, ore ?? OreCatalog.Veldspar);
  }

  public static Fleet CreateFleet() {
    return new Fleet([new Venture()], new SharedHoldStrategy());
  }

  public static PriceList CreatePrices() {
    return new PriceList(new Dictionary<Mineral, decimal> {
      [MineralCatalog.Tritanium] = 4m,
      [MineralCatalog.Pyerite] = 10m,
      [MineralCatalog.Mexallon] = 70m,
      [MineralCatalog.Isogen] = 150m,
    });
  }
}
