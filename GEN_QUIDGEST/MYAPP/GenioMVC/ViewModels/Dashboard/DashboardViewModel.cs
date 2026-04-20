using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.ViewModels.Dashboard
{
    public abstract class DashboardViewModel(UserContext userContext)
        : UserConfigurableInterfaceModel(userContext)
    {
        /// <summary>
        /// Widgets of this dashboard that the current user has access to.
        /// </summary>
        public List<Widget> Widgets { get; } = [];

        /// <summary>
        /// Registered singleton widget providers.
        /// </summary>
        protected Dictionary<WidgetType, WidgetProvider> SingletonWidgetProviders { get; set; }

        /// <summary>
        /// Registered widget providers.
        /// </summary>
        protected List<WidgetProvider> WidgetProviders { get; set; }

        /// <summary>
        /// Registered independent widget instances.
        /// </summary>
        protected List<Widget> IndependentWidgetInstances { get; set; }

        /// <summary>
        /// Loads the default dashboard configuration.
        /// </summary>
        private void LoadDefaultConfig()
        {
            Widgets.Clear();

            foreach (var singleton in SingletonWidgetProviders)
            {
                WidgetProvider provider = singleton.Value;
                provider.LoadInstances(m_userContext);
                Widgets.AddRange(provider.UserWidgets(m_userContext));
            }

            foreach (var provider in WidgetProviders)
            {
                provider.LoadInstances(m_userContext);
                Widgets.AddRange(provider.UserWidgets(m_userContext));
            }

            foreach (var widget in IndependentWidgetInstances)
            {
                if (!widget.ShowWidget)
                    continue;

                if(widget.UserHasAccess(m_userContext))
                    Widgets.Add(widget);
            }
        }

        /// <summary>
        /// Loads the dashboard configuration of the current user.
        /// </summary>
        public void Load()
        {
            CSGenio.framework.User user = m_userContext.User;
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            CSGenioAlstusr uiConfig = GetConfig();

            LoadDefaultConfig();

            // Retrieve the user configuration of the dashboard
            // If there is a user configuration, the original definition is ignored
            // and the position and visibility information of the user configuration is used
            List<CSGenioAusrwid> userWidgets = DashboardUiSettingsDbRec
                .Load(sp, uiConfig.ValDescric, user)
                .UserWidgets;

            // Apply user config if it exists
            if (userWidgets != null && userWidgets.Count > 0)
            {
                bool changes = false;

                foreach (CSGenioAusrwid userWidget in userWidgets)
                {
                    List<Widget> res = Widgets.Where(w => w.Id == userWidget.ValWidget).ToList();

                    Widget widget;
                    if (res.Count > 1)
                        widget = res.FirstOrDefault(w => w.Rowkey == userWidget.ValRowkey);
                    else
                        widget = res.FirstOrDefault();

                    if (widget != null)
                    {
                        widget.Hposition = userWidget.ValHposition;
                        widget.Vposition = userWidget.ValVposition;
                        widget.Visible = widget.Required || userWidget.ValVisible == 1;
                        widget.Rowkey = userWidget.ValRowkey;
                    }
                    else
                    {
                        // Widget is no longer available
                        // The widget might have been removed from the definition
                        // or the current user no longer has access to it
                        changes = true;

                        // Remove the widget from the user configuration
                        sp.openConnection();
                        userWidget.delete(sp);
                        sp.closeConnection();
                    }
                }

                if (changes)
                    DashboardUiSettingsDbRec.Invalidate(uiConfig.ValDescric, user);
            }
        }

        /// <summary>
        /// Saves the current dashboard configuration.
        /// </summary>
        public void Save(DashboardSaveRequest dto)
        {
            CSGenio.framework.User user = m_userContext.User;
            PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);
            CSGenioAlstusr lstusr = GetConfig();

            // Gets the current list of user widgets for this viewmodel
            List<CSGenioAusrwid> userWidgets = DashboardUiSettingsDbRec
                .Load(sp, lstusr.ValDescric, user)
                .UserWidgets;

            foreach (CSGenioAusrwid userWidget in userWidgets)
            {
                WidgetDto widget = !string.IsNullOrEmpty(userWidget.ValRowkey)
                    ? dto.Widgets.FirstOrDefault(w => w.Rowkey == userWidget.ValRowkey)
                    : widget = dto.Widgets.FirstOrDefault(w => w.Id == userWidget.ValWidget);

                if (widget == null)
                {
                    // Widget no longer exists in the configuration
                    sp.openConnection();
                    userWidget.delete(sp);
                    sp.closeConnection();
                }
                else
                {
                    // Update configuration
                    userWidget.ValWidget = widget.Id;
                    userWidget.ValRowkey = widget.Rowkey;
                    userWidget.ValVisible = widget.Visible ? 1 : 0;
                    userWidget.ValHposition = widget.Hposition;
                    userWidget.ValVposition = widget.Vposition;

                    sp.openConnection();
                    userWidget.update(sp);
                    sp.closeConnection();

                    dto.Widgets.Remove(widget);
                }
            }

            foreach (var widget in dto.Widgets)
            {
                // New widget configuration
                CSGenioAusrwid config =
                    new(user)
                    {
                        ValCodlstusr = lstusr.ValCodlstusr,
                        ValWidget = widget.Id,
                        ValRowkey = widget.Rowkey,
                        ValVisible = widget.Visible ? 1 : 0,
                        ValHposition = widget.Hposition,
                        ValVposition = widget.Vposition
                    };

                sp.openConnection();
                config.insert(sp);
                sp.closeConnection();
            }

            DashboardUiSettingsDbRec.Invalidate(lstusr.ValDescric, user);
        }

        /// <summary>
        /// Gets the widget with the provided type and identifier.
        /// </summary>
        /// <param name="type">Type of the widget.</param>
        /// <param name="id">The widget identifier.</param>
        public Widget GetWidget(WidgetType type, string id)
        {
            switch (type)
            {
                case WidgetType.Alert:
                case WidgetType.Menu:
                    return IndependentWidgetInstances.FirstOrDefault(w => w.Id == id);
                case WidgetType.Bookmark:
                    if (
                        SingletonWidgetProviders.TryGetValue(
                            WidgetType.Bookmark,
                            out WidgetProvider? value
                        )
                    )
                    {
                        var provider = value;
                        provider.LoadInstances(m_userContext);
                        return provider.GetInstance(id);
                    }
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the data for the provided widget type and identifier.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="BusinessException"></exception>
        public object GetWidgetData(WidgetType type, string id)
        {
            object data = null;
            Widget widget = GetWidget(type, id) ?? throw new InvalidOperationException($"Widget with id {id} not found");

            if (!widget.UserHasAccess(m_userContext))
                throw new BusinessException("Permission denied", "GetWidgetData", "Permission denied");

            User user = m_userContext.User;

            string ckey = string.Format(
                "{0}.{1}.{2}.{3}",
                Uuid,
                widget.Id,
                widget.Rowkey,
                user.Codpsw
            );

            if (widget.UsesCache)
                data = QCache.Instance.Dashboard.Get(ckey);

            if (data == null)
            {
                data = widget.GetData(m_userContext);

                if (data != null && widget.UsesCache)
                    QCache.Instance.Dashboard.Put(
                        ckey,
                        data,
                        TimeSpan.FromSeconds(widget.CacheTTL)
                    );
            }

            return data;
        }
    }
}
