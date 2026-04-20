/*****************************************************************
 *                                                               *
 * This store holds AI agents data,                              *
 * also defining functions to access and mutate it.              *
 *                                                               *
 *****************************************************************/

import { defineStore } from 'pinia'
import { ref } from 'vue'

type AgentData = {
	id: string
	prompt?: string
	systemPrompt?: string
	jsonSchema?: Record<string, unknown>
}

const state = () => ({
	// this will be a string array of agent IDs
	availableAgents: ref<string[]>([]),

	currentAgent: ref<AgentData | undefined>(undefined)
})

export const useAiDataStore = defineStore('aiData', {
	state: () => state(),
	getters: {
		/**
		 * Returns the current agent's ID or undefined if no agent is selected.
		 * @param state The current global state
		 */
		currentAgentId(state) {
			return state.currentAgent?.id
		},

		chatbotProxyUrl() {
			return 'chatbotapi'
		}
	},
	actions: {
		/**
		 * Sets the available AI agents.
		 * @param agents The list of available AI agents
		 */
		setAvailableAgents(agents: string[]) {
			this.availableAgents = agents
		},

		/**
		 * Sets the current AI agent.
		 * @param agent The current AI agent
		 */
		setCurrentAgent(agent: AgentData) {
			this.currentAgent = {
				...this.currentAgent,
				...agent
			}
		},

		/**
		 * Resets the auth data.
		 */
		resetStore() {
			Object.assign(this, state())
		}
	}
})
