using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class ImmutabilityTests {
  [Fact]
  public void FleetInputCollectionChangedPreservesOriginalShips() {
    var ships = new List<Ship> { new Venture() };
    var fleet = new Fleet(ships, new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));
    ships.Clear();
    ships.Add(new Epithal());

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(100m, completed.MinedVolume);
    Assert.Equal(3m, completed.WorkTime);
    Assert.Equal(3000m, completed.RentalCost);
  }

  [Fact]
  public void PriceListInputDictionaryChangedPreservesOriginalPrices() {
    var values = new Dictionary<Mineral, decimal> { [MineralCatalog.Tritanium] = 4m };
    var prices = new PriceList(values);
    var contract = new VolumeContract(TestData.CreateBelt(), TestData.CreateFleet(), prices, 100m);
    var calculator = new Calculator(new Station(0m));
    values[MineralCatalog.Tritanium] = 99m;

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(8000m, completed.Revenue);
    Assert.Equal(5000m, completed.Profit);
  }

  [Fact]
  public void OreInputDictionaryChangedPreservesOriginalYield() {
    var mineral = new Mineral("Test mineral");
    var yields = new Dictionary<Mineral, int> { [mineral] = 2 };
    var ore = new Ore("Test ore", 0.1m, yields);
    var prices = new PriceList(new Dictionary<Mineral, decimal> { [mineral] = 3m });
    var contract =
        new VolumeContract(TestData.CreateBelt(ore), TestData.CreateFleet(), prices, 100m);
    var calculator = new Calculator(new Station(0m));
    yields.Clear();

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(20L, completed.Minerals[mineral]);
    Assert.Equal(60m, completed.Revenue);
  }

  [Fact]
  public void CalculatorRepeatedCalculationStartsWithEmptyWarehouseAndHolds() {
    var contract = new VolumeContract(TestData.CreateBelt(OreCatalog.Pyroxeres),
                                      TestData.CreateFleet(), TestData.CreatePrices(), 1000m);
    var calculator = new Calculator(new Station(0m));
    calculator.Calculate(contract);

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(3, completed.TripCount);
    Assert.Equal(16m, completed.WorkTime);
    Assert.Equal(1000m, completed.MinedVolume);
    Assert.Equal(32L, completed.RemainingOreUnits);
    Assert.Equal(1485L, completed.Minerals[MineralCatalog.Pyerite]);
    Assert.Equal(495L, completed.Minerals[MineralCatalog.Mexallon]);
    Assert.Equal(33500m, completed.Profit);
  }

  [Fact]
  public void FleetExposedShipCollectionRejectsModification() {
    Fleet fleet = TestData.CreateFleet();

    if (fleet.Ships is IList<Ship> ships) {
      Assert.Throws<NotSupportedException>(() => ships[0] = new Epithal());
    }

    Assert.IsType<Venture>(fleet.Ships[0]);
    Assert.Equal(4m, fleet.Speed);
    Assert.Equal(1000m, fleet.HourlyRent);
  }

  [Fact]
  public void PriceListExposedPricesRejectsModification() {
    PriceList priceList = TestData.CreatePrices();

    if (priceList.Prices is IDictionary<Mineral, decimal> prices) {
      Assert.Throws<NotSupportedException>(() => prices[MineralCatalog.Tritanium] = 99m);
    }

    Assert.Equal(4m, priceList.Prices[MineralCatalog.Tritanium]);
  }

  [Fact]
  public void OreExposedYieldRejectsModification() {
    var ore = new Ore("Test ore", 0.1m,
                      new Dictionary<Mineral, int> { [MineralCatalog.Tritanium] = 200 });

    if (ore.MineralYield is IDictionary<Mineral, int> mineralYield) {
      Assert.Throws<NotSupportedException>(() => mineralYield.Clear());
    }

    Assert.Equal(200, ore.MineralYield[MineralCatalog.Tritanium]);
  }

  [Fact]
  public void CompletedContractExposedMineralsRejectsModification() {
    var contract = new VolumeContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                      TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));
    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));

    if (completed.Minerals is IDictionary<Mineral, long> minerals) {
      Assert.Throws<NotSupportedException>(() => minerals[MineralCatalog.Tritanium] = 0L);
    }

    Assert.Equal(2000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(8000m, completed.Revenue);
  }
}
