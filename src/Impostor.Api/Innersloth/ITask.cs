namespace Impostor.Api.Innersloth
{
    public interface ITask
    {
        int Id { get; }

        string Name { get; }

        TaskTypes Type { get; }

        TaskCategories Category { get; }

        bool IsVisual { get; }
    }
}
