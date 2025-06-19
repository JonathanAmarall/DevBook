namespace Infrastructure;

public interface IMappingConfigurationEntity
{
    Task InitializeAsync();
    public void Configure();
}
