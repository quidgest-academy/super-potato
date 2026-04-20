import { createRouter, createWebHashHistory } from 'vue-router'
import { loadLocaleMessages } from './plugins/i18n';
import QGlobal from './global';
import store from './store';
import EventBus from './utils/eventBus';

export function setupRouter(i18n) {
    const locale = i18n.mode === 'legacy' ? i18n.global.locale : i18n.global.locale.value;

    const routes = [
    {
        path: '/:pathMatch(.*)*',
        name: 'main',
        redirect: () => {
            var dSystem = store.getters.Year || QGlobal.defaultSystem;

            return {
                name: 'dashboard',
                params: {
                    culture: locale,
                    system: dSystem
                }
            };
        }
    },
    {
      path: '/:culture/:system/',
      name: 'main_params',
      redirect: {
        path: '/:culture/:system/Dashboard'
      },
      props: true
    },
    {
      path: '/:culture/:system/NoConfiguration',
      name: 'no_configuration',
      component: () => import(/* webpackChunkName: "no_configuration" */ './views/NoConfigSystem.vue'),
      props: true
    },
    {
      path: '/:culture/:system/Dashboard',
      name: 'dashboard',
      component: () => import(/* webpackChunkName: "dashboard" */ './views/Dashboard.vue'),
      props: true
    },
    {
      path: '/:culture/:system/Config',
      name: 'system_setup',
      component: () => import(/* webpackChunkName: "system_setup" */ './views/System_setup.vue'),
      props: true
    },
    {
      path: '/:culture/:system/AppConfig',
      name: 'app_configuration',
      component: () => import(/* webpackChunkName: "app_configuration" */ './views/App_configuration.vue'),
      props: true
    },
    {
      path: '/:culture/:system/ConfigMigration',
      name: 'config_migration',
      component: () => import(/* webpackChunkName: "config_migration" */ './views/Config_migration.vue'),
      props: true
    },
    {
      path: '/:culture/:system/DbAdmin/:tabId?', // The tabId is now a dynamic parameter (optional)
      name: 'maintenance',
      component: () => import(/* webpackChunkName: "maintenance" */ './views/Maintenance.vue'),
      props: true
    },
    {
      path: '/:culture/:system/DbAdmin/LogDetails/:logId?',
      name: 'log_details',
      component: () => import(/* webpackChunkName: "log_details" */ './views/Maintenance/LogDetails.vue'),
      props:  route => ({ logId: Number(route.params.logId) })
    },
    {
      path: '/:culture/:system/Users',
      name: 'users',
      component: () => import(/* webpackChunkName: "users" */ './views/Users/Users.vue'),
      props: true
    },
    {
      path: '/:culture/:system/Users/Roles',
      name: 'roles',
      component: () => import(/* webpackChunkName: "roles" */ './views/Users/RoleList.vue'),
      props: true
    },
    {
      path: '/:culture/:system/Users/RoleView/:module/:role',
      name: 'view_role',
      component: () => import(/* webpackChunkName: "viewrole" */ './views/Users/RoleView.vue'),
      props: true
    },
    {
      path: '/:culture/:system/ManageUsers/:mod/:cod?',
      name: 'manage_users',
      component: () => import(/* webpackChunkName: "manage_users" */ './views/Users/User_management.vue'),
      props: true
    },
    {
      path: '/:culture/:system/RoleAssign/:module/:role',
      name: 'assign_role',
      component: () => import(/* webpackChunkName: "roleassign" */ './views/Users/RoleAssign.vue'),
      props: true
    },
    {
      path: '/:culture/:system/ErrorLog',
      name: 'system_reports',
      component: () => import(/* webpackChunkName: "system_reports" */ './views/System_reports.vue'),
      props: true
    },
    {
      path: '/:culture/:system/ManageReports',
      name: 'report_management',
      component: () => import(/* webpackChunkName: "report_management" */ './views/Report_management.vue'),
      props: true
    },
    {
      path: '/:culture/:system/Notifications',
      name: 'notifications',
      component: () => import(/* webpackChunkName: "notifications" */ './views/Notifications.vue'),
      props: true
    },
    {
      path: '/:culture/:system/Email',
      name: 'email',
      component: () => import(/* webpackChunkName: "email" */ './views/Email.vue'),
      props: true
    },
    {
      path: '/:culture/:system/ManageNotif/:mod/:idnotif',
      name: 'manage_notif',
      component: () => import(/* webpackChunkName: "manage_notif" */ './views/Notifications/ManageNotif.vue'),
      props: true
    },
    {
      path: '/:culture/:system/ManageProperties/:mod/:codpmail?',
      name: 'manage_properties',
      component: () => import(/* webpackChunkName: "manage_properties" */ './views/Notifications/ManageProperties.vue'),
      props: true
    },
    {
        path: '/:culture/:system/ManageSignature/:mod/:codsigna?',
        name: 'manage_signature',
        component: () => import(/* webpackChunkName: "manage_signature" */ './views/Notifications/ManageSignature.vue'),
        props: true
    },
    {
        path: '/:culture/:system/ManageMessage/:mod/:idnotif/:codmesgs?',
        name: 'manage_message',
        component: () => import(/* webpackChunkName: "manage_message" */ './views/Notifications/ManageMessage.vue'),
        props: true
    },
  ];

    const router = createRouter({
        history: createWebHashHistory(),
        routes
    });

    // navigation guards
    router.beforeEach((to, from, next) => {
        var params = to.params;
        const locale = params.culture;
        const system = params.system;

        if (!$.isEmptyObject(system))
            EventBus.emit('SET_SYSTEM', system);

        // check locale
        if (!QGlobal.supportLocales.includes(locale)) {
            return false;
        }

        // load locale messages
        loadLocaleMessages(i18n, locale);

        next();
    });

    return router;
}
