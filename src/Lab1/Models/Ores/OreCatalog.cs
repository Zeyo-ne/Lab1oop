using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;

namespace Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;

public static class OreCatalog {
  public static Ore Veldspar { get; } =
      new("Veldspar", 0.1m, new Dictionary<Mineral, int> { [MineralCatalog.Tritanium] = 200 });

  public static Ore Scordite { get; } = new("Scordite", 0.15m,
                                            new Dictionary<Mineral, int> {
                                                [MineralCatalog.Tritanium] = 75,
                                                [MineralCatalog.Pyerite] = 55,
  });

  public static Ore Pyroxeres { get; } = new("Pyroxeres", 0.3m,
                                             new Dictionary<Mineral, int> {
                                                 [MineralCatalog.Pyerite] = 45,
                                                 [MineralCatalog.Mexallon] = 15,
  });

  public static Ore Kernite { get; } = new("Kernite", 1.2m,
                                           new Dictionary<Mineral, int> {
                                               [MineralCatalog.Mexallon] = 30,
                                               [MineralCatalog.Isogen] = 60,
  });
}
