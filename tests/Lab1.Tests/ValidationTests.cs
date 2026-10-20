using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class ValidationTests {
  private sealed class RestrictedContract : Contract {
    public RestrictedContract(AsteroidBelt belt, Fleet fleet, PriceList priceList)
        : base(belt, fleet, priceList) {
    }

    public override RejectedContract? Validate(decimal flightTime) {
      return new RejectedContract(RejectionReason.ContractConditionNotMet,
                                  "The station cannot accept this contract.");
    }

    public override bool CanStartTrip(decimal elapsedTime, decimal minedVolume,
                                      decimal flightTime) {
      return false;
    }

    public override bool CanStartCycle(decimal elapsedTime, decimal minedVolume,
                                       decimal flightTime) {
      return false;
    }
  }

  private sealed class TestShip : Ship {
    public TestShip(decimal capacity, decimal speed, decimal hourlyRent)
        : base("Test ship", capacity, speed, hourlyRent) {
    }

    public override decimal GetMiningVolume(int cycleNumber) {
      return 100m;
    }
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void OreWithNonPositiveUnitVolumeCannotBeCreated(int volume) {
    var mineralYield = new Dictionary<Mineral, int> { [MineralCatalog.Tritanium] = 200 };

    Assert.ThrowsAny<ArgumentException>(() => new Ore("Ore", volume, mineralYield));
  }

  [Theory]
  [InlineData(0, 4)]
  [InlineData(-1, 4)]
  [InlineData(400, 0)]
  [InlineData(400, -1)]
  public void ShipWithNonPositiveCapacityOrSpeedCannotBeCreated(int capacity, int speed) {
    const decimal hourlyRent = 1000m;

    Assert.ThrowsAny<ArgumentException>(() => new TestShip(capacity, speed, hourlyRent));
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void TimeContractWithNonPositiveDurationCannotBeCreated(int duration) {
    AsteroidBelt belt = TestData.CreateBelt();
    Fleet fleet = TestData.CreateFleet();
    PriceList prices = TestData.CreatePrices();

    Assert.ThrowsAny<ArgumentException>(() => new TimeContract(belt, fleet, prices, duration));
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void VolumeContractWithNonPositiveTargetCannotBeCreated(int targetVolume) {
    AsteroidBelt belt = TestData.CreateBelt();
    Fleet fleet = TestData.CreateFleet();
    PriceList prices = TestData.CreatePrices();

    Assert.ThrowsAny<ArgumentException>(() =>
                                            new VolumeContract(belt, fleet, prices, targetVolume));
  }

  [Theory]
  [InlineData(-1)]
  [InlineData(2)]
  public void StationWithTaxRateOutsideUnitIntervalCannotBeCreated(int taxRate) {
    Assert.ThrowsAny<ArgumentException>(() => new Station(taxRate));
  }

  [Fact]
  public void MineralWithSameNameHasValueEquality() {
    var first = new Mineral("Tritanium");
    var second = new Mineral("Tritanium");
    var prices = new Dictionary<Mineral, decimal> { [first] = 4m };

    Assert.Equal(4m, prices[second]);
    Assert.Equal(first, second);
  }

  [Fact]
  public void ContractWithCustomRejectionPreservesItsReason() {
    var contract = new RestrictedContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                          TestData.CreatePrices());
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));
    Assert.Equal(RejectionReason.ContractConditionNotMet, rejected.Reason);
    Assert.Equal("The station cannot accept this contract.", rejected.Description);
  }

  [Fact]
  public void FleetWithNullShipCannotBeCreated() {
    Ship[] ships = [null!];
    var strategy = new SharedHoldStrategy();

    Assert.ThrowsAny<ArgumentException>(() => new Fleet(ships, strategy));
  }

  [Fact]
  public void OreWithoutMineralYieldCannotBeCreated() {
    var mineralYield = new Dictionary<Mineral, int>();

    Assert.ThrowsAny<ArgumentException>(() => new Ore("Ore", 0.1m, mineralYield));
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void OreWithNonPositiveMineralYieldCannotBeCreated(int quantity) {
    var mineralYield = new Dictionary<Mineral, int> { [MineralCatalog.Tritanium] = quantity };

    Assert.ThrowsAny<ArgumentException>(() => new Ore("Ore", 0.1m, mineralYield));
  }

  [Fact]
  public void PriceListWithNegativePriceCannotBeCreated() {
    var prices = new Dictionary<Mineral, decimal> { [MineralCatalog.Tritanium] = -0.01m };

    Assert.ThrowsAny<ArgumentException>(() => new PriceList(prices));
  }

  [Fact]
  public void AsteroidBeltWithNegativeDistanceCannotBeCreated() {
    const decimal distance = -0.01m;

    Assert.ThrowsAny<ArgumentException>(
        () => new AsteroidBelt("Belt", distance, OreCatalog.Veldspar));
  }

  [Fact]
  public void ShipWithNegativeRentCannotBeCreated() {
    const decimal hourlyRent = -0.01m;

    Assert.ThrowsAny<ArgumentException>(() => new TestShip(400m, 4m, hourlyRent));
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void CargoHoldWithNonPositiveCapacityCannotBeCreated(int capacity) {
    Assert.ThrowsAny<ArgumentException>(() => new CargoHold(capacity));
  }

  [Theory]
  [InlineData(-1)]
  [InlineData(101)]
  public void StationWithFractionalRateOutsideUnitIntervalCannotBeCreated(int taxPercent) {
    decimal taxRate = taxPercent / 100m;

    Assert.ThrowsAny<ArgumentException>(() => new Station(taxRate));
  }

  [Fact]
  public void ContractWithMissingPriceAndNoMiningCapacityReportsMissingPrice() {
    var fleet = new Fleet([new Epithal()], new SharedHoldStrategy());
    var prices = new PriceList(new Dictionary<Mineral, decimal> {
      [MineralCatalog.Tritanium] = 4m,
    });
    var contract = new TimeContract(TestData.CreateBelt(OreCatalog.Scordite), fleet, prices, 1m);
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));

    Assert.Equal(RejectionReason.MissingMineralPrice, rejected.Reason);
    Assert.Equal("The price list does not contain a price for Pyerite.", rejected.Description);
  }

  [Fact]
  public void ContractWithNoMiningCapacityAndInsufficientTimeReportsNoMiningCapacity() {
    var fleet = new Fleet([new Epithal()], new SharedHoldStrategy());
    var contract = new TimeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 1m);
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));

    Assert.Equal(RejectionReason.NoMiningCapacity, rejected.Reason);
    Assert.Equal("No ship can mine a complete first-cycle yield with empty holds.",
                 rejected.Description);
  }
}
