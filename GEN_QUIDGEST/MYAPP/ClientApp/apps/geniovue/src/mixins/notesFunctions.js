/**
 * @typedef {Object} NoteContext
 * @property {string} uid Context identifier used by the API.
 * @property {(string|null)} key Optional record key within the context.
 */

/**
 * Builds the notes context for the provided route.
 *
 * Route metadata is expected to provide a `routeType` and, depending on the type,
 * additional metadata:
 * - `form`: expects `route.name` to start with `Form_` (or equivalent prefix) and may contain `route.params.id`
 * - `menu`: expects `meta.module` and `meta.order`
 * - `home`: expects `meta.module`
 *
 * @param {import('vue-router').RouteLocationNormalized} route - Active route.
 * @returns {NoteContext|null} Context for the API, or null when the route has no note context.
 */
export function getNoteContext(route)
{
	const meta = route.meta
	if(!meta) return null

	if(meta.routeType === 'form')
	{
		// Convention: route name contains the form identifier after a fixed prefix.
		const formName = String(route.name ?? '').slice(5)
		const recordId = route.params.id ?? null

		return {
			uid: `F__${formName}`,
			key: recordId
		}
	}
	else if(meta.routeType === 'menu')
	{
		return {
			uid: `M__${meta.module}_${meta.order}`,
			key: null
		}
	}
	else if(meta.routeType === 'home')
	{
		return {
			uid: `HP__${meta.module}`,
			key: null
		}
	}
	else return null
}

export default {
	getNoteContext
}
