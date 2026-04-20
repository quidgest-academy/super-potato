<template>
	<div class="page__wrapper">
		<article>
			<div>
				<header>
					<h1 class="pageTitle">
						{{ texts.accountConfirmError }}
					</h1>
				</header>

				<div class="card-wrapper">
					<p>
						{{ texts.contactAdmin }}
					</p>
				</div>
			</div>
		</article>
	</div>
</template>

<script>
	import { computed } from 'vue'

	import hardcodedTexts from '@/hardcodedTexts.js'

	export default {
		name: 'TicketConfirmError',

		expose: [],

		data()
		{
			return {
				texts: {
					accountConfirmError: computed(() => this.Resources[hardcodedTexts.accountConfirmError]),
					contactAdmin: computed(() => this.Resources[hardcodedTexts.contactAdmin])
				}
			}
		}
	}
</script>