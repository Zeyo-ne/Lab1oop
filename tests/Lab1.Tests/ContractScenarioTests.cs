using Itmo.ObjectOrientedProgramming.Lab1.Contracts;
using Itmo.ObjectOrientedProgramming.Lab1.Models;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;
using Itmo.ObjectOrientedProgramming.Lab1.Models.Ores;
using Itmo.ObjectOrientedProgramming.Lab1.Results;
using Itmo.ObjectOrientedProgramming.Lab1.Ships;
using Itmo.ObjectOrientedProgramming.Lab1.Storage;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab1.Tests;

public class ContractScenarioTests {
  [Fact(DisplayName = "Сценарий 1. Контракт на срок")]
  public void TimeContractWithSingleVentureIsCompleted() {
    var contract = new TimeContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                    TestData.CreatePrices(), 6m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(1, completed.TripCount);
    Assert.Equal(6m, completed.WorkTime);
    Assert.Equal(400m, completed.MinedVolume);
    Assert.Equal(8000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(0L, completed.RemainingOreUnits);
    Assert.Equal(32000m, completed.Revenue);
    Assert.Equal(6000m, completed.RentalCost);
    Assert.Equal(0m, completed.Tax);
    Assert.Equal(26000m, completed.Profit);
  }

  [Fact(DisplayName = "Сценарий 2. Пустой флот")]
  public void FleetWithoutShipsCannotBeCreated() {
    Ship[] ships = [];
    var strategy = new SharedHoldStrategy();

    Assert.Throws<ArgumentException>(() => new Fleet(ships, strategy));
  }

  [Fact(DisplayName = "Сценарий 3. Контракт на объём")]
  public void VolumeContractWithSingleVentureIsCompleted() {
    var contract = new VolumeContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                      TestData.CreatePrices(), 1000m);
    var calculator = new Calculator(new Station(0m));

    CompletedContract completed = Assert.IsType<CompletedContract>(calculator.Calculate(contract));
    Assert.Equal(3, completed.TripCount);
    Assert.Equal(16m, completed.WorkTime);
    Assert.Equal(1000m, completed.MinedVolume);
    Assert.Equal(20000L, completed.Minerals[MineralCatalog.Tritanium]);
    Assert.Equal(0L, completed.RemainingOreUnits);
    Assert.Equal(80000m, completed.Revenue);
    Assert.Equal(16000m, completed.RentalCost);
    Assert.Equal(0m, completed.Tax);
    Assert.Equal(64000m, completed.Profit);
  }

  [Fact(DisplayName = "Сценарий 4. Отклонение по сроку")]
  public void TimeContractTooShortIsRejected() {
    var contract = new TimeContract(TestData.CreateBelt(), TestData.CreateFleet(),
                                    TestData.CreatePrices(), 2m);
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));
    Assert.Equal(RejectionReason.InsufficientTime, rejected.Reason);
    Assert.False(string.IsNullOrWhiteSpace(rejected.Description));
  }

  [Fact(DisplayName = "Сценарий 5. Отклонение по прайс-листу")]
  public void ContractWithIncompletePriceListIsRejected() {
    var prices =
        new PriceList(new Dictionary<Mineral, decimal> { [MineralCatalog.Tritanium] = 4m });
    var contract = new TimeContract(TestData.CreateBelt(OreCatalog.Scordite),
                                    TestData.CreateFleet(), prices, 6m);
    var calculator = new Calculator(new Station(0m));

    RejectedContract rejected = Assert.IsType<RejectedContract>(calculator.Calculate(contract));
    Assert.Equal(RejectionReason.MissingMineralPrice, rejected.Reason);
    Assert.Contains("Pyerite", rejected.Description, StringComparison.Ordinal);
  }
}
