using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class StorageStrategyTests {
  [Theory]
  [InlineData(true, 1, 9, 13500, 26500)]
  [InlineData(false, 2, 13, 19500, 20500)]
  public void FleetWithTransporterUsesCapacityAccordingToStorageStrategy(
      bool shared, int expectedTrips, int expectedTime, int expectedRent, int expectedProfit) {
    IStorageStrategy strategy;
    if (shared) {
      strategy = new SharedHoldStrategy();
    } else {
      strategy = new SeparateHoldsStrategy();
    }

    var fleet = new Fleet([new Venture(), new Epithal()], strategy);
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 500m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(expectedTrips, completed.TripCount);
    Assert.Equal(expectedTime, completed.WorkTime);
    Assert.Equal(500m, completed.MinedVolume);
    Assert.Equal(expectedRent, completed.RentalCost);
    Assert.Equal(expectedProfit, completed.Profit);
  }

  [Fact]
  public void SharedStorageWithPartialOwnHoldFillsOwnHoldFirst() {
    CargoHold[] holds = [new CargoHold(3000m), new CargoHold(400m), new CargoHold(3000m)];
    Assert.True(holds[1].TryLoad(350m));
    var strategy = new SharedHoldStrategy();

    Assert.True(strategy.TryPlace(holds, 1, 100m));
    Assert.Equal(50m, holds[0].LoadedVolume);
    Assert.Equal(400m, holds[1].LoadedVolume);
    Assert.Equal(0m, holds[2].LoadedVolume);
  }

  [Fact]
  public void SharedStorageInsufficientTotalCapacityLeavesAllHoldsUnchanged() {
    CargoHold[] holds = [new CargoHold(3000m), new CargoHold(400m), new CargoHold(3000m)];
    Assert.True(holds[0].TryLoad(3000m));
    Assert.True(holds[1].TryLoad(350m));
    Assert.True(holds[2].TryLoad(2980m));
    var strategy = new SharedHoldStrategy();

    Assert.False(strategy.TryPlace(holds, 1, 100m));
    Assert.Equal(3000m, holds[0].LoadedVolume);
    Assert.Equal(350m, holds[1].LoadedVolume);
    Assert.Equal(2980m, holds[2].LoadedVolume);
  }

  [Fact]
  public void SeparateStorageInsufficientOwnCapacityDoesNotUseOtherHolds() {
    CargoHold[] holds = [new CargoHold(400m), new CargoHold(3000m)];
    Assert.True(holds[0].TryLoad(350m));
    var strategy = new SeparateHoldsStrategy();

    Assert.False(strategy.TryPlace(holds, 0, 100m));
    Assert.Equal(350m, holds[0].LoadedVolume);
    Assert.Equal(0m, holds[1].LoadedVolume);
  }

  [Theory]
  [InlineData(60, 25)]
  [InlineData(65, 30)]
  public void SharedStorageWithFractionalLoadFillsOtherHoldsInFleetOrder(int load,
                                                                         int lastHoldLoad) {
    CargoHold[] holds = [new CargoHold(0.15m), new CargoHold(0.2m), new CargoHold(0.3m)];
    var strategy = new SharedHoldStrategy();

    Assert.True(strategy.TryPlace(holds, 1, load / 100m));
    Assert.Equal(0.15m, holds[0].LoadedVolume);
    Assert.Equal(0.2m, holds[1].LoadedVolume);
    Assert.Equal(lastHoldLoad / 100m, holds[2].LoadedVolume);
  }

  [Fact]
  public void SeparateStorageLoadEqualsOwnCapacityFillsOnlyOwnHold() {
    CargoHold[] holds = [new CargoHold(0.2m), new CargoHold(0.3m)];
    var strategy = new SeparateHoldsStrategy();

    Assert.True(strategy.TryPlace(holds, 0, 0.2m));
    Assert.Equal(0.2m, holds[0].LoadedVolume);
    Assert.Equal(0m, holds[1].LoadedVolume);
  }
}
