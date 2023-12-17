namespace Impostor.Api.Innersloth.Maps;

public sealed class TaskData
{
    internal TaskData(int id, TaskTypes type, TaskCategories category, bool isVisual = false)
    {
        Id = id;
        Name = id.ToString();
        Type = type;
        Category = category;
        IsVisual = isVisual;
    }

    public int Id { get; }

    public string Name { get; }

    public TaskTypes Type { get; }

    public TaskCategories Category { get; }

    public bool IsVisual { get; }
}
