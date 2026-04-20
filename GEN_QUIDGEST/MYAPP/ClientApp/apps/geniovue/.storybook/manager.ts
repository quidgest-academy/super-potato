import { addons } from '@storybook/manager-api'
import { create } from '@storybook/theming/create'

const myTheme = create({
	base: 'light',
	brandTitle: 'Genio Proto',
	brandUrl: 'https://genio.quidgest.com',
	brandImage: 'genio-dark.png',
	brandTarget: '_self'
})

addons.setConfig({
	theme: myTheme
})
