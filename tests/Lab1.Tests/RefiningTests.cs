using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class RefiningTests {
  private sealed class FractionalMiner : Ship {
    public FractionalMiner() : base("Fractional miner", 0.75m, 4m, 100m) {
    }

    public override decimal GetMiningVolume(int cycleNumber) {
      return 0.75m;
    }
  }

  [Fact]
  public void RefiningWithMultipleTripsRoundsPerTripAndCarriesRemainingUnits() {
    var contract = new VolumeContract(TestData.CreateBelt(OreCatalog.Pyroxeres),
                                      TestData.CreateFleet(), TestData.CreatePrices(), 1000m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(3, completed.TripCount);
    Assert.Equal(16m, completed.WorkTime);
    Assert.Equal(1000m, completed.MinedVolume);
    Assert.Equal(32L, completed.RemainingOreUnits);
    Assert.Equal(1485L, completed.Minerals[MineralCatalog.Pyerite]);
    Assert.Equal(495L, completed.Minerals[MineralCatalog.Mexallon]);
    Assert.Equal(49500m, completed.Revenue);
    Assert.Equal(16000m, completed.RentalCost);
    Assert.Equal(33500m, completed.Profit);
  }

  [Fact]
  public void ScorditeAfterRefiningProducesBothMineralsAndRemainingOre() {
    var contract = new VolumeContract(TestData.CreateBelt(OreCatalog.Scordite),
                                      TestData.CreateFleet(), TestData.CreatePrices(), 400m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(6m, completed.WorkTime);
    Assert.Equal(400m, completed.MinedVolume);
    Assert.Equal(66L, completed.RemainingOreUnits);
    Assert.Equal(1950L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(1430L, completed.Minerals[MineralCatalog.Pyerite]);
    Assert.Equal(22100m, completed.Revenue);
    Assert.Equal(6000m, completed.RentalCost);
    Assert.Equal(16100m, completed.Profit);
  }

  [Fact]
  public void KerniteAfterRefiningProducesBothMineralsAndRemainingOre() {
    var contract = new VolumeContract(TestData.CreateBelt(OreCatalog.Kernite),
                                      TestData.CreateFleet(), TestData.CreatePrices(), 400m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(6m, completed.WorkTime);
    Assert.Equal(400m, completed.MinedVolume);
    Assert.Equal(33L, completed.RemainingOreUnits);
    Assert.Equal(90L, completed.Minerals[MineralCatalog.Mexallon]);
    Assert.Equal(180L, completed.Minerals[MineralCatalog.Isogen]);
    Assert.Equal(33300m, completed.Revenue);
    Assert.Equal(6000m, completed.RentalCost);
    Assert.Equal(27300m, completed.Profit);
  }

  [Theory]
  [InlineData(0, 0, 5000)]
  [InlineData(5, 400, 4600)]
  [InlineData(100, 8000, -3000)]
  public void TaxOnRevenueAllowsNegativeProfit(int taxPercent, int expectedTax,
                                               int expectedProfit) {
    var contract = new VolumeContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                      TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(taxPercent / 100m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(8000m, completed.Revenue);
    Assert.Equal(3000m, completed.RentalCost);
    Assert.Equal(expectedTax, completed.Tax);
    Assert.Equal(expectedProfit, completed.Profit);
  }

  [Fact]
  public void RefiningLessThanOnePortionLeavesOreUnsold() {
    var contract = new VolumeContract(TestData.CreateBelt(OreCatalog.Kernite),
                                      TestData.CreateFleet(), TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(100m, completed.MinedVolume);
    Assert.Equal(83L, completed.RemainingOreUnits);
    Assert.All(completed.Minerals.Values, quantity => Assert.Equal(0L, quantity));
    Assert.Equal(0m, completed.Revenue);
    Assert.Equal(-3000m, completed.Profit);
  }

  [Fact]
  public void CustomOreWithFractionalPricesPreservesMoneyPrecision() {
    var mineral = new Mineral("Moonite");
    var ore = new Ore("Moonstone", 0.7m, new Dictionary<Mineral, int> { [mineral] = 3 });
    var prices = new PriceList(new Dictionary<Mineral, decimal> { [mineral] = 1.2345m });
    var contract =
        new VolumeContract(TestData.CreateBelt(ore), TestData.CreateFleet(), prices, 100m);
    var calculator = new Calculator(new Station(0.015m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(3L, completed.Minerals[mineral]);
    Assert.Equal(42L, completed.RemainingOreUnits);
    Assert.Equal(3.7035m, completed.Revenue);
    Assert.Equal(0.0555525m, completed.Tax);
    Assert.Equal(-2996.3520525m, completed.Profit);
  }

  [Fact]
  public void RefiningMultipleShipsInOneTripRoundsCombinedVolume() {
    var ore =
        new Ore("Test ore", 1m, new Dictionary<Mineral, int> { [MineralCatalog.Tritanium] = 1 });
    var fleet =
        new Fleet([new FractionalMiner(), new FractionalMiner()], new SeparateHoldsStrategy());
    var contract =
        new VolumeContract(TestData.CreateBelt(ore), fleet, TestData.CreatePrices(), 1.5m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(3m, completed.WorkTime);
    Assert.Equal(1.5m, completed.MinedVolume);
    Assert.Equal(1L, completed.RemainingOreUnits);
    Assert.Equal(0L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(0m, completed.Revenue);
    Assert.Equal(-600m, completed.Profit);
  }

  [Fact]
  public void RefiningLessThanOneUnitPerTripDiscardsFractionAfterEachTrip() {
    var ore =
        new Ore("Test ore", 1m, new Dictionary<Mineral, int> { [MineralCatalog.Tritanium] = 1 });
    var fleet = new Fleet([new FractionalMiner()], new SharedHoldStrategy());
    var contract =
        new VolumeContract(TestData.CreateBelt(ore), fleet, TestData.CreatePrices(), 1.5m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(2, completed.TripCount);
    Assert.Equal(6m, completed.WorkTime);
    Assert.Equal(1.5m, completed.MinedVolume);
    Assert.Equal(0L, completed.RemainingOreUnits);
    Assert.Equal(0L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(0m, completed.Revenue);
    Assert.Equal(-600m, completed.Profit);
  }

  [Fact]
  public void RefiningMineralCountExceedsIntRangePreservesQuantityAndRevenue() {
    var ore = new Ore("Rich ore", 0.001m,
                      new Dictionary<Mineral, int> {
                        [MineralCatalog.Tritanium] = 3_000_000,
                      });
    var contract = new VolumeContract(TestData.CreateBelt(ore), TestData.CreateFleet(),
                                      TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(3_000_000_000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(0L, completed.RemainingOreUnits);
    Assert.Equal(12_000_000_000m, completed.Revenue);
    Assert.Equal(11_999_997_000m, completed.Profit);
  }
}
