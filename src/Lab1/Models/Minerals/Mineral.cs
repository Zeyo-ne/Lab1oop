namespace Itmo.ObjectOrientedProgramming.Lab1.Models.Minerals;

public sealed record Mineral {
  public string Name { get; }

  public Mineral(string name) {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Name = name;
  }
}
