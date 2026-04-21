// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { updateQueryParams } from './routeUtils.js'

export default function getMenusRoutes()
{
	return [
		{
			path: '/:culture/:system/FOR/menu/FOR_3211',
			name: 'menu-FOR_3211',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_3211/QMenuFor3211.vue'),
			beforeEnter: [updateQueryParams],
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '3211',
				baseArea: 'CTAX',
				hasInitialPHE: false,
				humanKeyFields: [],
				limitations: ['city' /* DB */],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/FOR/menu/FOR_321',
			name: 'menu-FOR_321',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_321/QMenuFor321.vue'),
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '321',
				baseArea: 'CITY',
				hasInitialPHE: false,
				humanKeyFields: ['ValCity'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/FOR/menu/FOR_21',
			name: 'menu-FOR_21',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_21/QMenuFor21.vue'),
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '21',
				baseArea: 'PROPE',
				hasInitialPHE: false,
				humanKeyFields: ['ValTitle', 'ValPrice'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/FOR/menu/FOR_411',
			name: 'menu-FOR_411',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_411/QMenuFor411.vue'),
			beforeEnter: [updateQueryParams],
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '411',
				baseArea: 'PROPE',
				hasInitialPHE: false,
				humanKeyFields: ['ValTitle', 'ValPrice'],
				limitations: ['agent' /* DB */],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/FOR/menu/FOR_511',
			name: 'menu-FOR_511',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_511/QMenuFor511.vue'),
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '511',
				baseArea: 'PROPE',
				hasInitialPHE: false,
				humanKeyFields: ['ValTitle', 'ValPrice'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/FOR/menu/FOR_311',
			name: 'menu-FOR_311',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_311/QMenuFor311.vue'),
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '311',
				baseArea: 'COUNT',
				hasInitialPHE: false,
				humanKeyFields: ['ValCountry'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/FOR/menu/FOR_11',
			name: 'menu-FOR_11',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_11/QMenuFor11.vue'),
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '11',
				baseArea: 'AGENT',
				hasInitialPHE: false,
				humanKeyFields: ['ValName', 'ValEmail'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/FOR/menu/FOR_41',
			name: 'menu-FOR_41',
			component: () => import('@/views/menus/ModuleFOR/MenuFOR_41/QMenuFor41.vue'),
			meta: {
				routeType: 'menu',
				module: 'FOR',
				order: '41',
				baseArea: 'AGENT',
				hasInitialPHE: false,
				humanKeyFields: ['ValName', 'ValEmail'],
				isPopup: false
			}
		},
	]
}
