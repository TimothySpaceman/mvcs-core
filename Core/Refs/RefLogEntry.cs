using Core.Storage;

namespace Core.Refs;

public record ReflogEntry(
    string RefKey,
    HashId OldValue,
    HashId NewValue,
    DateTimeOffset Timestamp,
    string Message
);