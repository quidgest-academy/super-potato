<template>
    <row>
        <q-card
            width="block"
            class="q-card--admin-default"
            :title="resources.auditTitle">
            <q-row-container>
                <q-checkbox
                    v-model="model.RegistLoginOut"
                    :label="resources.auditLoginLabel" />
                <q-checkbox
                    v-model="model.RegistActions"
                    :label="resources.auditActionsLabel" />
                <q-checkbox
                    v-model="model.AuditInterface"
                    :label="resources.auditSystemLabel" />
                <q-checkbox
                    v-model="model.EventTracking"
                    :label="resources.eventTrackingLabel" />
            </q-row-container>
        </q-card>
    </row>
</template>

<script>
    import { reusableMixin } from '@/mixins/mainMixin';
	import { QUtils } from '@/utils/mainUtils';

    export default {
		name: 'audit',

        mixins: [reusableMixin],

		emits: ['alert-class'],

        props: {
			model: {
				required: true
			},
			resources: {
				type: Object,
				required: true
			}
		},
    }
</script>
