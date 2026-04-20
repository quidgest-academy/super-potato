namespace GenioMVC.Models.Navigation
{
    /// <summary>
    /// Represents the intention to call a controller action
    /// </summary>
    public class ItemActionDescriptor
    {
        public string Action { get; set; }
        public string Resource { get; set; }
        public string Id { get; set; }
        public string Nav { get; set; }
    }
}