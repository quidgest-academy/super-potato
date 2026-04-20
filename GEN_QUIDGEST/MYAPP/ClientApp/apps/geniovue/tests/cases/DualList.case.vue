<template>
	<div>
		<h2>Dual List Example</h2>
		<q-dual-list
			v-model="selectedItems"
			:items="allItems"
			@selected-items="handleSelectedItems" />
	</div>
</template>

<script>
	import QDualList from '@/components/rendering/QDualList.vue'

	export default {
		name: 'QDualListContainer',

		components: {
			QDualList
		},

		expose: [],

		data()
		{
			return {
				allItems: [
					{ id: 1, name: 'Variable 001: Comprehensive Analysis of Quantum Field Theory and Its Implications on Modern Physics' },
					{ id: 2, name: 'Variable 002: Exploring the Nuances of Black Hole Physics and Interstellar Anomalies' },
					{ id: 3, name: 'Variable 003: The Intricate Web of Cellular Mechanisms and Molecular Interactions in Human Physiology' },
					{ id: 4, name: 'Variable 004: A Detailed Study of Postmodern Literature and Its Reflection on Societal Changes' },
					{ id: 5, name: 'Variable 005: Comparative Anthropological Examination of Indigenous Cultures Across Continents' },
					{ id: 6, name: 'Variable 006: A Deep Dive into the Aesthetics and Principles of Brutalist and Neo-Futurist Architecture' },
					{ id: 7, name: 'Variable 007: Delving into the Dynasties: An In-depth Historical Account of Ancient Empires and Their Legacies' },
					{ id: 8, name: 'Variable 008: From Farm to Table: Sustainable Practices and Innovations in Modern Agriculture' },
					{ id: 9, name: 'Variable 009: Zen, Tao, and Dharma: Philosophical Tenets of the Eastern Spiritual Traditions' },
					{ id: 10, name: 'Variable 010: Renaissance and Beyond: The Artists, Their Visions, and the Transformation of European Art' },
					{ id: 11, name: 'Variable 011: Uncharted Depths: Marine Biology and the Wonders of the World Beneath Waves' },
					{ id: 12, name: 'Variable 012: Algorithms, Neural Networks, and the Future Landscape of Artificial Intelligence' },
					{ id: 13, name: 'Variable 013: Digital Echo Chambers: Understanding the Psychological Effects of Modern Online Platforms' },
					{ id: 14, name: 'Variable 014: The Symphony of a Century: Tracing Orchestral Evolutions from Bach to Beethoven' },
					{ id: 15, name: 'Variable 015: The Semiotics and Syntax of Evolving Human Communication Methods' },
					{ id: 16, name: 'Variable 016: Geopolitics and the Shifting Paradigms of Power in the Contemporary World' },
					{ id: 17, name: 'Variable 017: Stellar Expeditions: Pioneering Research on Galaxies, Stars, and Planetary Systems' },
					{ id: 18, name: 'Variable 018: A Gastronomic Journey: Traditions, Innovations, and the Fusion of Global Culinary Arts' },
					{ id: 19, name: 'Variable 019: Steam, Steel, and Progress: The Pivotal Moments of the Industrial Era' },
					{ id: 20, name: 'Variable 020: Quantum to Cosmic: Bridging the Scale from Subatomic to Intergalactic Realms' },
					{ id: 21, name: 'Variable 021: Socratic Dialogues to Stoic Teachings: Wisdom of the Ages and Timeless Philosophical Debates' }
				],
				selectedItems: [2] // By default, "Item 2" is selected
			}
		},

		methods: {
			getItemNameById(id)
			{
				const item = this.allItems.find((i) => i.id === id)
				return item ? item.name : ''
			},

			handleSelectedItems(selectedArray)
			{
				// eslint-disable-next-line no-console
				console.log(selectedArray)
			}
		}
	}
</script>
