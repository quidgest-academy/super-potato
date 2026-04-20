import ViewModelBase from '@/mixins/viewModelBase.js'

/**
 * Extends ViewModelBase to provide menu-specific functionalities.
 * This class introduces a `dataApi` property with a getter for extra properties.
 */
export default class MenuViewModelBase extends ViewModelBase
{
	/**Creates an instance of MenuViewModelBase.
	 * @param {Object} vueContext - The Vue instance context.
	 * @param {Object} options - The options object for configuration.
	 */
	constructor(vueContext, options)
	{
		super(vueContext, options)

		// Defining a non-enumerable, configurable, and writable property 'dataApi'.
		Object.defineProperty(this, 'dataApi', {
			value: {
				// Getter for extraProperties from the base class.
				get extraProperties() {
					return this.extraProperties;
				}
			},
			enumerable: false,
			configurable: true,
			writable: true
		})
	}
}