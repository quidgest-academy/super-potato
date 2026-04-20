import { createFramework } from '@quidgest/ui/framework'

const framework = createFramework({
	defaults: {
		QCheckbox: {
			icons: {
				checked: {
					icon: 'check-bold'
				},

				indeterminate: {
					icon: 'minus-thick'
				}
			}
		},

		QIcon: {
			type: 'svg'
		},

		QIconSvg: {
			bundle: 'Content/svgbundle.svg'
		}
	}
})

export default framework
