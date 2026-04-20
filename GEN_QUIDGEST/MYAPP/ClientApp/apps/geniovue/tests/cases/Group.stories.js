import Group from '@/components/containers/GroupBoxContainer.vue'

export default {
	title: 'Containers/Group',
	component: Group,
	tags: ['autodocs'],
	args: {
		specialClasses: ''
	},
	argTypes: {
		specialClasses: {
			type: 'select',
			options: [
				'default',
				'c-groupbox--background',
				'c-groupbox--minor',
				'c-groupbox--minor-border-top',
				'c-groupbox--title-background',
				'c-groupbox--subsection'],
			description: 'Special classes that can be applied to this control'
		}
	},
	parameters: {
		docs: {
			description: {
				component: 'This component is used to aggregate different information in a coherent way'
			}
		}
	}
}

export const Simple = {
	args: {
		label: 'My title'
	},

	render: (args) => ({
		components: { Group },

		setup()
		{
			return { args }
		},
		template: '<QGroupBoxContainer :class="args.specialClasses" v-bind="args"><p>Example of a group box</p></QGroupBoxContainer>'
	})
}

export const NoTitle = {
	args: {
		label: ''
	},
	render: (args) => ({
		components: { Group },
		setup()
		{
			return { args }
		},
		template:
			'<QGroupBoxContainer :class="args.specialClasses" v-bind="minor"><p>Example of a group box without a title</p></QGroupBoxContainer>'
	})
}

/**
 * Groups are an hierarchical element. They can contain any kind of element, including themselves.
 */
export const GroupInsideGroup = {
	args: {
		label: 'My title'
	},

	render: (args) => ({
		components: { Group },
		setup()
		{
			return { args }
		},
		template: `
			<QGroupBoxContainer :class="args.specialClasses" v-bind="args">
				<p>Top-level group</p>
				<QGroupBoxContainer :class="args.specialClasses" v-bind="args">
					<p>Mid level group</p>
					<QGroupBoxContainer :class="args.specialClasses" v-bind="args">
						<p>Leaf group</p>
					</QGroupBoxContainer>
				</QGroupBoxContainer>
			</QGroupBoxContainer>`
	})
}
