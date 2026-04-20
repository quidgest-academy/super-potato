<template>
	<div class="q-markdown-editor__toolbar">
		<template v-for="btn in filteredButtons">
			<q-button
				v-if="!btn.options"
				class="q-markdown-editor__toolbar-btn"
				borderless
				:key="btn.name"
				:data-control-type="btn.name"
				@click="() => handleAction(btn.name)"
				:title="btn.description"
				:disabled="disabled">
				<q-icon
					:icon="btn.icon"
					size="20" />
			</q-button>
			<template v-else>
				<q-button
					class="q-markdown-editor__toolbar-btn q-markdown-editor__toolbar-btn--options"
					borderless
					:key="btn.name"
					:data-control-type="btn.name"
					:title="btn.description"
					:disabled="disabled"
					:ref="(el) => (buttonsRef[btn.name] = el)">
					<q-icon
						:icon="btn.icon"
						size="20" />
				</q-button>
				<q-dropdown-menu
					:key="`${btn.name}-options`"
					:items="btn.options"
					:activator="buttonsRef[btn.name]?.$el"
					placement="bottom-start"
					@select="(item) => handleAction(btn.name, item)" />
			</template>
		</template>
	</div>
</template>

<script>
	import { ref } from 'vue'

	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'
	import { MarkdownOptions, DEFAULT_TEXTS } from './markdown.js'
	import { QDropdownMenu } from '@quidgest/ui/components'

	export default {
		name: 'QMarkdownEditorToolbar',

		emits: [
			'action'
		],

		components: {
			QDropdownMenu
		},

		expose: [],

		props: {
			/**
			 * Options for Markdown (which may disable certain toolbar buttons)
			 */
			options: {
				type: Object,
				default: () => new MarkdownOptions()
			},

			/**
			 * Whether the toolbar is disabled.
			 */
			disabled: {
				type: Boolean,
				default: false
			},

			/**
			 * Necessary strings to be used in labels and buttons.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		data() {
			return {
				/**
				 * References to elements in multiple values scenario.
				 */
				buttonsRef: ref({})
			}
		},

		computed: {
			/**
			 * Return only those buttons which are available.
			 */
			filteredButtons() {
				/*
					Definition of all toolbar buttons
					Each object should include:
						- name
						- description
						- icon
						- enable (optional)
				*/
				const buttons = [
					{
						name: 'applyBold',
						description: this.texts.addBoldText + ' (Ctrl + B)',
						icon: 'format-bold',
					},
					{
						name: 'applyItalic',
						description: this.texts.addItalicText + ' (Ctrl + I)',
						icon: 'format-italic',
					},
					{
						name: 'applyHeading',
						description: this.texts.addHeadingText,
						icon: 'format-size',
						options: [
							{
								key: 1,
								//description: 'H1',
								icon: { icon: 'format-header-1' },
							},
							{
								key: 2,
								//description: 'H2',
								icon: { icon: 'format-header-2' },
							},
							{
								key: 3,
								//description: 'H3',
								icon: { icon: 'format-header-3' },
							},
							{
								key: 4,
								//description: 'H4',
								icon: { icon: 'format-header-4' },
							},
							{
								key: 5,
								//description: 'H5',
								icon: { icon: 'format-header-5' },
							},
							{
								key: 6,
								//description: 'H6',
								icon: { icon: 'format-header-6' },
							}
						]
					},
					{
						name: 'applyStrikethrough',
						description: this.texts.addStrikethroughText + ' (Ctrl + Shift + X)',
						icon: 'format-strikethrough-variant',
					},
					{
						name: 'applyQuote',
						description: this.texts.insertQuote,
						icon: 'format-quote-close',
					},
					{
						name: 'applyCodeBlock',
						description: this.texts.insertCodeBlock,
						icon: 'code-block-tags',
					},
					{
						name: 'applyLink',
						description: this.texts.addLink + ' (Ctrl + K)',
						icon: 'link-variant',
					},
					{
						name: 'applyBulletList',
						description: this.texts.addBulletList,
						icon: 'format-list-bulleted',
					},
					{
						name: 'applyNumberedList',
						description: this.texts.addNumberedList,
						icon: 'format-list-numbered',
					},
					{
						name: 'applyChecklist',
						description: this.texts.addCheckList,
						icon: 'format-list-checks',
						enable: this.options.allowCheckList
					},
					{
						name: 'applyHorizontalRule',
						description: this.texts.addHorizontalRule + ' (Ctrl + M)',
						icon: 'minus',
					},
					{
						name: 'applyTable',
						description: this.texts.addTable,
						icon: 'table',
					},
					{
						name: 'insertImageMarkdown',
						description: this.texts.addImage,
						icon: 'image',
						enable: this.options.allowImage
					}
				]

				return buttons.filter(btn => btn.enable !== false)
			}
		},

		methods: {
			handleAction(action, params) {
				if(!this.disabled)
					this.$emit('action', { event: action, params })
			}
		}
	}
</script>
