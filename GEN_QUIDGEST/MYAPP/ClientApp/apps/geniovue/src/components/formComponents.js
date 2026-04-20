import { defineAsyncComponent } from 'vue'

export default {
	install: (app) => {
		app.component('QFormAccountInfo', defineAsyncComponent(() => import('@/views/shared/AccountInfo.vue')))
		app.component('QFormAgent', defineAsyncComponent(() => import('@/views/forms/FormAgent/QFormAgent.vue')))
		app.component('QFormAlbum', defineAsyncComponent(() => import('@/views/forms/FormAlbum/QFormAlbum.vue')))
		app.component('QFormCity', defineAsyncComponent(() => import('@/views/forms/FormCity/QFormCity.vue')))
		app.component('QFormContact', defineAsyncComponent(() => import('@/views/forms/FormContact/QFormContact.vue')))
		app.component('QFormCountry', defineAsyncComponent(() => import('@/views/forms/FormCountry/QFormCountry.vue')))
		app.component('QFormProperty', defineAsyncComponent(() => import('@/views/forms/FormProperty/QFormProperty.vue')))
	}
}
