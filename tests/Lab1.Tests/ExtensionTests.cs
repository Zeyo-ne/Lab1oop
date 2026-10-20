using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class ExtensionTests {
  private sealed class AcceleratingMiner : Ship {
    public AcceleratingMiner() : base("Accelerating miner", 500m, 4m, 100m) {
    }

    public override decimal GetMiningVolume(int cycleNumber) {
      return cycleNumber * 40m;
    }
  }

  private sealed class LastHoldStrategy : IStorageStrategy {
    public bool TryPlace(IReadOnlyList<CargoHold> holds, int miningShipIndex, decimal volume) {
      return holds[holds.Count - 1].TryLoad(volume);
    }
  }

  private sealed class SingleCycleContract : Contract {
    public SingleCycleContract(AsteroidBelt belt, Fleet fleet, PriceList priceList)
        : base(belt, fleet, priceList) {
    }

    public override bool CanStartTrip(decimal elapsedTime, decimal minedVolume,
                                      decimal flightTime) {
      return minedVolume == 0m;
    }

    public override bool CanStartCycle(decimal elapsedTime, decimal minedVolume,
                                       decimal flightTime) {
      return minedVolume == 0m;
    }
  }

  [Fact]
  public void CalculatorWithCustomShipUsesItsMiningBehavior() {
    var fleet = new Fleet([new AcceleratingMiner()], new SharedHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 100m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(120m, completed.MinedVolume);
    Assert.Equal(4m, completed.WorkTime);
    Assert.Equal(2400L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(400m, completed.RentalCost);
    Assert.Equal(9200m, completed.Profit);
  }

  [Fact]
  public void CalculatorWithCustomStorageStrategyUsesItsPlacement() {
    var fleet = new Fleet([new Venture(), new Epithal()], new LastHoldStrategy());
    var contract = new VolumeContract(TestData.CreateBelt(), fleet, TestData.CreatePrices(), 500m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(500m, completed.MinedVolume);
    Assert.Equal(9m, completed.WorkTime);
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(13500m, completed.RentalCost);
  }

  [Fact]
  public void CalculatorWithCustomContractUsesItsCompletionRules() {
    var contract = new SingleCycleContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                           TestData.CreatePrices());
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(100m, completed.MinedVolume);
    Assert.Equal(3m, completed.WorkTime);
    Assert.Equal(5000m, completed.Profit);
  }
}
