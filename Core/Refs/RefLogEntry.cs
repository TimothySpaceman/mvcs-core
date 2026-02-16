using Core.Storage;

namespace Core.Refs;

public record ReflogEntry(
    HashId OldValue,
    HashId NewValue,
    DateTimeOffset Timestamp,
    string Message
);