namespace SharedKernel;
public abstract record PagedRequest
{
    public short? PageNumber { get; init; } = 1;
    public short? PageSize { get; init; } = 10;
}
