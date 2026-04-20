import { createStore } from 'vuex'

const store = createStore({
	state: {
		currentApp: '',
		currentYear: '',
		multiYearStatus: false, // True if the application has more than one data system, false otherwise
		currentLanguage: '',
		availableYears: [],
		defaultYear: ''
	},
	mutations: {
		SET_APP(state, newValue) {
			state.currentApp = newValue
		},
		SET_YEAR(state, newValue) {
			state.currentYear = newValue
		},
		SET_YEARS(state, years) {
			state.availableYears = years.map((y) => ({ Text: y, Value: y }))
		},
		SET_DEFAULT_YEAR(state, year) {
			state.defaultYear = year
		},
		SET_MULTIYEARSTATUS(state, newValue) {
			state.multiYearStatus = newValue
		},
		SET_LANGUAGE(state, newValue) {
			state.currentLanguage = newValue
		}
	},
	actions: {
		changeApp(context, newValue) {
			context.commit('SET_APP', newValue)
		},
		changeYear(context, newValue) {
			context.commit('SET_YEAR', newValue)
		},
		changeMultiYearStatus(context, newValue) {
			context.commit('SET_MULTIYEARSTATUS', newValue)
		},
		changeLanguage(context, newValue) {
			context.commit('SET_LANGUAGE', newValue)
		},
		setYears(context, years) {
			context.commit('SET_YEARS', years)
		},
		setDefaultYear(context, year) {
			context.commit('SET_DEFAULT_YEAR', year)
		}
	},
	getters: {
		App: (state) => state.currentApp,
		Year: (state) => state.currentYear,
		MultiYearStatus: (state) => state.multiYearStatus,
		Language: (state) => state.currentLanguage,
		Years: (state) => state.availableYears,
		DefaultYear: (state) => state.defaultYear
	}
})

export default store
