import _find from 'lodash-es/find'
import { computed } from 'vue'

import hardcodedTexts from '@/hardcodedTexts.js'
import formControlClass from '@/mixins/formControl.js'
import FormHandlers from '@/mixins/formHandlers.js'
import { isVisibleColumn } from '@/mixins/listFunctions'

/*****************************************************************
 * This mixin defines methods to be reused by editable table     *
 * list forms.                                                   *
 *****************************************************************/
export default {
	emits: {
		'mark-for-deletion': () => true,
		'undo-deletion': () => true,
		'toggle-errors': () => true
	},

	mixins: [FormHandlers],

	props: {
		/**
		 * The initial state of the editable table list row.
		 */
		initialState: {
			type: String,
			default: ''
		},

		/**
		 * Array containing column definitions.
		 */
		columns: {
			type: Array,
			required: true
		},

		/**
		 * Is deleted state mode
		 * it was necessary because when you navigate to different form and come back we need to now if the row was deleted before.
		 * if it was, the state will be "Deleted" and the undo button will appear.
		 */
		isDeletedState: {
			type: Boolean,
			default: false
		}
	},

	data()
	{
		return {
			currentRouteParams: {},

			formButtons: new formControlClass.FormControlButtons(),

			texts: {
				delete: computed(() => this.Resources[hardcodedTexts.delete]),
				remove: computed(() => this.Resources[hardcodedTexts.remove]),
				restore: computed(() => this.Resources[hardcodedTexts.restore]),
				messages: computed(() => this.Resources[hardcodedTexts.messages])
			}
		}
	},

	methods: {
		/**
		 * Determine if column is visible
		 * @param {string} area The column table
		 * @param {string} field The column field
		 * @returns {Boolean} If column is visible
		 */
		canShowColumn(area, field)
		{
			const column = _find(this.columns, { area, field })
			return isVisibleColumn(column)
		}
	}
}
