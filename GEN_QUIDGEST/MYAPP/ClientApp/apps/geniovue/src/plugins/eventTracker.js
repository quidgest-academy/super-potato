import { mapActions } from 'pinia'

import { useTracingDataStore } from '@quidgest/clientapp/stores'

export default {
	install(app)
	{
		app.config.globalProperties.$eventTracker = {
			...mapActions(useTracingDataStore, [
				'addTrace',
				'addWarning',
				'addError'
			])
		}

		if (import.meta.env.PROD)
		{
			// Global handler for uncaught errors propagating from within the application
			app.config.errorHandler = (err, instance, info) => {
				const tracing = useTracingDataStore()
				tracing.addError({
					origin: 'Global errorHandler',
					message: 'Unhandled error',
					contextData: {
						err,
						name: `Instance component: ${instance?._?.type?.name}`,
						info
					}
				})
			}
		}

		// Custom handler for runtime warnings from Vue
		// TODO: Do we want it or not?
		/*
		app.config.warnHandler = (msg, instance, trace) => {
			const tracing = useTracingDataStore()
			tracing.addWarning({
				origin: 'Global warnHandler',
				message: msg,
				contextData: {
					instance,
					trace
				}
			})
		}
		*/
	}
}
