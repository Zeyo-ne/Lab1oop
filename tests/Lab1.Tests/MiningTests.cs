using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class MiningTests {
  private sealed class IntermittentMiner : Ship {
    public IntermittentMiner() : base("Intermittent miner", 500m, 4m, 100m) {
    }

    public override decimal GetMiningVolume(int cycleNumber) {
      if (cycleNumber % 2 == 0) {
        return 0m;
      }

      return 50m;
    }
  }

  private sealed class SmallHoldMiner : Ship {
    public SmallHoldMiner() : base("Small hold miner", 50m, 4m, 100m) {
    }

    public override decimal GetMiningVolume(int cycleNumber) {
      return 100m;
    }
  }

  [Fact]
  public void ProspectOnEachTripStartsWithReducedMiningVolume() {
    var fleet = new Fleet([new Prospect()], new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 1000m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(2, completed.TripCount);
    Assert.Equal(1050m, completed.MinedVolume);
    Assert.Equal(11.2m, completed.WorkTime);
    Assert.Equal(21000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(84000m, completed.Revenue);
    Assert.Equal(16800m, completed.RentalCost);
    Assert.Equal(67200m, completed.Profit);
  }

  [Theory]
  [InlineData(true, 1375, 9, 27400, 109600, 22500, 87100)]
  [InlineData(false, 1325, 8, 26400, 105600, 20000, 85600)]
  public void SharedStorageWithMultipleMinersUsesFleetOrder(bool ventureFirst, int expectedVolume,
                                                            int expectedTime, long expectedMinerals,
                                                            int expectedRevenue, int expectedRent,
                                                            int expectedProfit) {
    Ship[] ships;
    if (ventureFirst) {
      ships = [new Venture(), new Prospect()];
    } else {
      ships = [new Prospect(), new Venture()];
    }

    var fleet = new Fleet(ships, new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 1300m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(expectedVolume, completed.MinedVolume);
    Assert.Equal(expectedTime, completed.WorkTime);
    Assert.Equal(expectedMinerals, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(50L, completed.RemainingOreUnits);
    Assert.Equal(expectedRevenue, completed.Revenue);
    Assert.Equal(expectedRent, completed.RentalCost);
    Assert.Equal(expectedProfit, completed.Profit);
  }

  [Fact]
  public void VolumeContractTargetReachedDuringCycleStillProcessesRemainingShips() {
    var fleet = new Fleet([new Venture(), new Venture()], new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 50m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(200m, completed.MinedVolume);
    Assert.Equal(3m, completed.WorkTime);
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(4000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(16000m, completed.Revenue);
    Assert.Equal(6000m, completed.RentalCost);
    Assert.Equal(10000m, completed.Profit);
  }

  [Theory]
  [InlineData(3, 0, 0, 0)]
  [InlineData(9, 1, 6, 400)]
  public void TimeContractMinimumReturnTimeEqualsDeadlineDoesNotStartTrip(int duration,
                                                                          int expectedTrips,
                                                                          int expectedTime,
                                                                          int expectedVolume) {
    var contract = new TimeContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                    TestData.CreatePrices(), duration);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(expectedTrips, completed.TripCount);
    Assert.Equal(expectedTime, completed.WorkTime);
    Assert.Equal(expectedVolume, completed.MinedVolume);
    Assert.Equal(expectedTime * 1000m, completed.RentalCost);
  }

  [Fact]
  public void TimeContractCycleReturnsAtDeadlineCompletesCycle() {
    var contract = new TimeContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                    TestData.CreatePrices(), 4m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(4m, completed.WorkTime);
    Assert.Equal(200m, completed.MinedVolume);
    Assert.Equal(1, completed.TripCount);
  }

  [Fact]
  public void FleetWithOnlyTransportersIsRejected() {
    var fleet = new Fleet([new Epithal()], new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));
    Assert.Equal(RejectionReason.NoMiningCapacity, rejected.Reason);
    Assert.False(string.IsNullOrWhiteSpace(rejected.Description));
  }

  [Fact]
  public void SeparateStorageFirstCycleDoesNotFitIsRejected() {
    var fleet = new Fleet([new SmallHoldMiner(), new Epithal()], new SeparateHoldsStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));
    Assert.Equal(RejectionReason.NoMiningCapacity, rejected.Reason);
  }

  [Fact]
  public void SharedStorageFirstCycleNeedsTransporterIsCompleted() {
    var fleet = new Fleet([new SmallHoldMiner(), new Epithal()], new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(100m, completed.MinedVolume);
    Assert.Equal(5m, completed.WorkTime);
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(3000m, completed.RentalCost);
  }

  [Fact]
  public void TimeContractWithFractionalFlightTimePreservesTimeAndRentPrecision() {
    var belt = new AsteroidBelt("Nearby belt", 1m, OreCatalog.Veldspar);
    var contract = new TimeContract(belt, TestData.CreateFleet(), TestData.CreatePrices(), 2.5m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(2.5m, completed.WorkTime);
    Assert.Equal(200m, completed.MinedVolume);
    Assert.Equal(4000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(16000m, completed.Revenue);
    Assert.Equal(2500m, completed.RentalCost);
    Assert.Equal(13500m, completed.Profit);
  }

  [Fact]
  public void TimeContractWithSmallTimeMarginDoesNotRoundDuration() {
    var belt = new AsteroidBelt("Nearby belt", 1m, OreCatalog.Veldspar);
    var contract = new TimeContract(belt, TestData.CreateFleet(), TestData.CreatePrices(), 1.5001m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(1.5m, completed.WorkTime);
    Assert.Equal(100m, completed.MinedVolume);
    Assert.Equal(1500m, completed.RentalCost);
    Assert.Equal(6500m, completed.Profit);
  }

  [Fact]
  public void VolumeContractWithZeroDistanceChargesOnlyMiningTime() {
    var belt = new AsteroidBelt("Station belt", 0m, OreCatalog.Veldspar);
    var contract = new VolumeContract(belt, TestData.CreateFleet(), TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(1m, completed.WorkTime);
    Assert.Equal(100m, completed.MinedVolume);
    Assert.Equal(1000m, completed.RentalCost);
    Assert.Equal(7000m, completed.Profit);
  }

  [Fact]
  public void SharedStorageFirstYieldExceedsTotalCapacityIsRejected() {
    var fleet = new Fleet([new SmallHoldMiner()], new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));
    Assert.Equal(RejectionReason.NoMiningCapacity, rejected.Reason);
  }

  [Fact]
  public void MiningZeroYieldCycleEndsTripWithoutChargingAnotherHour() {
    var fleet = new Fleet([new IntermittentMiner()], new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(2, completed.TripCount);
    Assert.Equal(6m, completed.WorkTime);
    Assert.Equal(100m, completed.MinedVolume);
    Assert.Equal(2000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(600m, completed.RentalCost);
    Assert.Equal(7400m, completed.Profit);
  }
}
