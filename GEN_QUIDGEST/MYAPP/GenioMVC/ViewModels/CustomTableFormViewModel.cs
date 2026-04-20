using System.Collections.Specialized;
using GenioMVC.Models;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
    /// <summary>
    /// Represents the base class for custom table form view models.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    public abstract class CustomTableFormViewModel<T> : FormViewModel<T> where T : ModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTableFormViewModel{T}"/> class.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        /// <param name="identifier">The identifier (optional).</param>
        /// <param name="nestedForm">Indicates if this is a nested form.</param>
        protected CustomTableFormViewModel(UserContext userContext, string? identifier = null, bool nestedForm = false)
            : base(userContext, identifier, nestedForm) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTableFormViewModel{T}"/> class.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="row">The data row.</param>
        /// <param name="nestedForm">Indicates if this is a nested form.</param>
        protected CustomTableFormViewModel(UserContext userContext, string identifier, T row, bool nestedForm = false)
            : base(userContext, identifier, row, nestedForm) { }

        /// <summary>
        /// Loads all the information needed to present the form in insert mode.
        /// </summary>
        public override void NewLoad()
        {
            LoadPartial(new NameValueCollection());
            LoadDefaultValues();
        }
    }
}
