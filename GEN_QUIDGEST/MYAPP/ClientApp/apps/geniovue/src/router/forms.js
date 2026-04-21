import { propsConverter } from './routeUtils.js'

export default function getFormsRoutes()
{
	return [
		{
			path: '/:culture/:system/:module/form/AGENT/:mode/:id?',
			name: 'form-AGENT',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormAgent/QFormAgent.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'AGENT',
				humanKeyFields: ['ValName', 'ValEmail'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/:module/form/ALBUM/:mode/:id?',
			name: 'form-ALBUM',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormAlbum/QFormAlbum.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'PHOTO',
				humanKeyFields: ['ValTitle'],
				isPopup: true
			}
		},
		{
			path: '/:culture/:system/:module/form/CITY/:mode/:id?',
			name: 'form-CITY',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormCity/QFormCity.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'CITY',
				humanKeyFields: ['ValCity'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/:module/form/CONTACT/:mode/:id?',
			name: 'form-CONTACT',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormContact/QFormContact.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'CONTA',
				humanKeyFields: ['ValClient'],
				isPopup: true
			}
		},
		{
			path: '/:culture/:system/:module/form/COUNTRY/:mode/:id?',
			name: 'form-COUNTRY',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormCountry/QFormCountry.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'COUNT',
				humanKeyFields: ['ValCountry'],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/:module/form/CTAX/:mode/:id?',
			name: 'form-CTAX',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormCtax/QFormCtax.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'CTAX',
				humanKeyFields: [],
				isPopup: false
			}
		},
		{
			path: '/:culture/:system/:module/form/PROPERTY/:mode/:id?',
			name: 'form-PROPERTY',
			props: route => propsConverter(route),
			component: () => import('@/views/forms/FormProperty/QFormProperty.vue'),
			meta: {
				routeType: 'form',
				baseArea: 'PROPE',
				humanKeyFields: ['ValTitle', 'ValPrice'],
				isPopup: false
			}
		},
	]
}
