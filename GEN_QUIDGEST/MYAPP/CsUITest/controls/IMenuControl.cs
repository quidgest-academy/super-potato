namespace quidgest.uitests.controls;

public interface IMenuControl
{
    void ActivateMenu(string moduleId, string itemId);

    void ActivateModule(string moduleId);

    void ActivateBookmark(string moduleId, string itemId);

    void AddBookmark(string moduleId, string itemId);

    void RemoveBookmark(string moduleId, string itemId);

    public bool HasBookmark(string moduleId, string itemId);

    public int GetMenuCount(string moduleId, string itemId);

    public int GetBookmarkCount();
}
