using StudentPortal.Api.Domain.Housing;

namespace StudentPortal.Api.Dtos;

public record UpsertHousingDto(
    HousingStatus Status,
    string? AddressLine,
    string? City,
    DateTime? MoveInDate,
    bool ContractUploaded,
    string? Notes
);