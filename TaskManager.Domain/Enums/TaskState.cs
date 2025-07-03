using System.Text.Json.Serialization;

namespace TaskManager.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskState
{
    Waiting,
    InProgress,
    Completed
}
