<template>
	<div
		password-meter
		class="password-meter">
		<meter
			max="4"
			password-strength-meter
			:value="scoreStrength">
			{{ strengthText }}
		</meter>
		<p password-strength-text></p>
	</div>
</template>

<script>
	import _isEmpty from 'lodash-es/isEmpty'

	import { validateTexts } from '@quidgest/clientapp/utils/genericFunctions'

	// The texts needed by the component.
	const DEFAULT_TEXTS = {
		strong: 'Strong',
		good: 'Good',
		weak: 'Weak',
		poor: 'Poor'
	}

	export default {
		name: 'QPasswordMeter',

		props: {
			/**
			 * The password input value used to calculate the password strength.
			 */
			inputValue: String,

			/**
			 * Text labels for different levels of password strength.
			 */
			texts: {
				type: Object,
				validator: (value) => validateTexts(DEFAULT_TEXTS, value),
				default: () => DEFAULT_TEXTS
			}
		},

		expose: [],

		data()
		{
			return {
				strengthText: '',
				scoreStrength: 0
			}
		},

		methods: {
			/**
			 * Calculates the score of the given password based on certain criteria.
			 * @param {String} pass - The password to be evaluated.
			 * @returns {Number} The calculated score of the password.
			 */
			scorePassword(pass)
			{
				let score = 0
				if (!pass)
					return score

				// Award every unique letter until 5 repetitions.
				const letters = new Object()
				for (let i = 0; i < pass.length; i++)
				{
					letters[pass[i]] = (letters[pass[i]] || 0) + 1
					score += 5.0 / letters[pass[i]]
				}

				// Bonus points for mixing it up.
				const variations = {
					digits: /\d/.test(pass),
					lower: /[a-z]/.test(pass),
					upper: /[A-Z]/.test(pass),
					nonWords: /\W/.test(pass)
				}
				let variationCount = 0

				for (const check in variations)
					variationCount += (variations[check] === true) ? 1 : 0
				score += (variationCount - 1) * 10

				return parseInt(score)
			},

			/**
			 * Updates the displayed strength text based on the evaluated strength of the password.
			 * @param {String} textValue - The current value of the password input.
			 */
			updateStrengthText(textValue)
			{
				const score = _isEmpty(textValue) ? 0 : this.scorePassword(textValue)

				if (_isEmpty(textValue))
				{
					this.scoreStrength = 0
					this.strengthText = ''
				}
				else if (score > 80)
				{
					this.scoreStrength = 4
					this.strengthText = this.texts.strong
				}
				else if (score > 60)
				{
					this.scoreStrength = 3
					this.strengthText = this.texts.good
				}
				else if (score >= 30)
				{
					this.scoreStrength = 2
					this.strengthText = this.texts.weak
				}
				else if (score <= 30)
				{
					this.scoreStrength = 1
					this.strengthText = this.texts.poor
				}
			}
		},

		watch: {
			inputValue(newValue)
			{
				this.updateStrengthText(newValue)
			}
		}
	}
</script>
