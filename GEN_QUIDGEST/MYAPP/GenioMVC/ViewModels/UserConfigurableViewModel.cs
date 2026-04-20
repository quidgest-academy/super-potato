using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels
{
    /// <summary>
    /// Represents a view model of an interface
    /// that can be configured by the user.
    /// </summary>
    public abstract class UserConfigurableInterfaceModel(UserContext userContext)
        : ViewModelBase(userContext)
    {
        /// <summary>
        /// Gets the unique user interface descriptor.
        /// </summary>
        public abstract string Uuid { get; }

        /// <summary>
        /// Gets the user configuration or creates one if it does not exist yet.
        /// </summary>
        /// <returns></returns>
        public CSGenioAlstusr GetConfig()
        {
            User user = m_userContext.User;
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

            CSGenioAlstusr model = CSGenioAlstusr
                .searchList(
                    sp,
                    user,
                    CriteriaSet
                        .And()
                        .Equal(CSGenioAlstusr.FldDescric, Uuid)
                        .Equal(CSGenioAlstusr.FldCodpsw, user.Codpsw)
                        .Equal(CSGenioAlstusr.FldZzstate, 0)
                )
                .FirstOrDefault();

            // Create lstusr if it does not exist yet
            if (model == null)
            {
                model = new CSGenioAlstusr(user)
                {
                    ValCodpsw = user.Codpsw,
                    ValModulo = user.CurrentModule,
                    ValSistema = Configuration.Program,
                    ValDescric = Uuid
                };

                sp.openConnection();
                model.insert(sp);
                sp.closeConnection();

                UserUiSettings.Invalidate(model.ValDescric, user);
            }

            return model;
        }
    }
}
