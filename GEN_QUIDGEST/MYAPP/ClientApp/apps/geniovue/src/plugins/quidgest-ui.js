import { createFramework } from '@quidgest/ui/framework'

const framework = createFramework({
	themes: {
		defaultTheme: 'Light',
		themes: [
			{
				name: 'Light',
				mode: 'light',
				colors: {
					secondary: '#C9B793',
					primaryLight: '#F3904A',
					primary: '#D69E98',
					primaryDark: '#A34C29',
				}
			}
		]
	},
	defaults: {
		QIconSvg: {
			bundle: 'Content/svgbundle.svg?v=25'
		},
		QCollapsible: {
			icons: {
				chevron: {
					icon: 'expand'
				}
			}
		},
		QListItem: {
			icons: {
				check: {
					icon: 'ok'
				},
				description: {
					icon: 'help'
				}
			}
		},
		QSelect: {
			itemValue: 'key',
			itemLabel: 'value',
			icons: {
				clear: {
					icon: 'close'
				},
				chevron: {
					icon: 'expand'
				}
			}
		},
		QCombobox: {
			itemValue: 'key',
			itemLabel: 'value',
			icons: {
				clear: {
					icon: 'close'
				},
				chevron: {
					icon: 'expand'
				}
			}
		},
		QPropertyList: {
			icons: {
				open: {
					icon: 'square-minus',
				},
				close: {
					icon: 'square-plus',
				}
			}
		},
		QCheckbox: {
			icons: {
				checked: {
					icon: 'ok'
				},
				indeterminate: {
					icon: 'minus'
				}
			}
		},
		QCarousel: {
			icons: {
				back: {
					icon: 'step-back'
				},
				forward: {
					icon: 'step-forward'
				}
			}
		}
	}
})

export default framework
