import { useSystemDataStore } from '@quidgest/clientapp/stores'
import { useAuthDataStore } from '@quidgest/clientapp/stores'

export default function getUserRoutes()
{
	const systemDataStore = useSystemDataStore()

	return [
		{
			path: '/:culture/:system/:module/Register/:id?',
			name: 'user-register-full-path',
			meta: {
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: false,
				isPublicPage: true,
				noBreadcrumbs: true
			},
			component: () => import('@/views/shared/Register.vue')
		},
		{
			path: '/Register/:id?',
			name: 'user-register',
			redirect: (to) => {
				return {
					name: 'user-register-full-path',
					params: {
						culture: to.params.culture ? to.params.culture : systemDataStore.system.currentLang,
						system: to.params.system ? to.params.system : systemDataStore.system.currentSystem,
						module: to.params.module ? to.params.module : systemDataStore.system.currentModule,
						id: to.params.id
					}
				}
			}
		},
		{
			path: '/:culture/CreationSuccess',
			name: 'creation-success',
			meta: {
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: false,
				isPublicPage: true,
				noBreadcrumbs: true,
				isFullScreenPage: true
			},
			component: () => import('@/views/shared/AccountCreationSuccess.vue')
		},
		{
			path: '/:culture/SuccessTicketConfirm',
			name: 'success-ticket-confirm',
			meta: {
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: false,
				isPublicPage: true,
				noBreadcrumbs: true,
				isFullScreenPage: true
			},
			component: () => import('@/views/shared/TicketConfirmSuccess.vue')
		},
		{
			path: '/:culture/ErrorTicketConfirm',
			name: 'error-ticket-confirm',
			meta: {
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: false,
				isPublicPage: true,
				noBreadcrumbs: true,
				isFullScreenPage: true
			},
			component: () => import('@/views/shared/TicketConfirmError.vue')
		},
		{
			path: '/:culture/:system/:module/RecoverPassword',
			name: 'password-recovery',
			meta: {
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: false,
				isPublicPage: true,
				noBreadcrumbs: true,
				isFullScreenPage: true
			},
			component: () => import('@/views/shared/RecoverPassword.vue')
		},
		{
			path: '/:culture/:system/:module/RecoverPasswordChange',
			name: 'password-recovery-change',
			meta: {
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: false,
				isPublicPage: true,
				noBreadcrumbs: true,
				isFullScreenPage: true
			},
			component: () => import('@/views/shared/RecoverPasswordChange.vue')
		},
		{
			path: '/:culture/:system/:module/RecoverPasswordChangeSuccess',
			name: 'password-recovery-change-success',
			meta: {
				module: 'Public',
				hasInitialPHE: false,
				isHomePage: false,
				isPublicPage: true,
				noBreadcrumbs: true,
				isFullScreenPage: true
			},
			component: () => import('@/views/shared/RecoverPasswordChangeSuccess.vue')
		},
		{
			path: '/:culture/:system/Profile',
			name: 'profile',
			meta: {
				hasInitialPHE: false
			},
			component: () => import('@/views/shared/Profile.vue'),
			beforeEnter: () => {
				const authDataStore = useAuthDataStore()
				if (!authDataStore.hasUserSettings)
				{
					// reject the navigation
					return false
				}
			}
		},
		{
			path: '/:culture/:system/Change2FA',
			name: 'change2fa',
			meta: {
				hasInitialPHE: false
			},
			component: () => import('@/views/shared/Change2FA.vue')
		}
	]
}
