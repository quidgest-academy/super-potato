<template>
    <row>
        <qtable 
			:rows="tNotifsOnBD.rows"
			:columns="tNotifsOnBD.columns"
			:config="tNotifsOnBD.config"
			:totalRows="tNotifsOnBD.total_rows"
			style="overflow-x: auto;">
        </qtable>
    </row>
    <row>
    <q-button
        :label="Resources.CANCELAR49513"
        @click="cancel" />
    </row>
</template>

<script>
    import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';
	import bootbox from 'bootbox';

    export default {
		name: "notifsDB",
		emits: ['update-model'],
		mixins: [reusableMixin],
		props: {
			model: {
				required: true
			},
			tNotifsOnBD: {
				required: true,
				type: Object
			}
		},
		data() {
			return {
				Model: {}
			}
		},
		methods: {
            cancel() {
				this.$router.replace({ name: 'notifications', params: { culture: this.currentLang, system: this.currentYear } });
			}
		}
	}
</script>
