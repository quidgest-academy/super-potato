import ProgressBarControl from '@/components/rendering/QProgress.vue'

export default {
	title: 'Inputs/QProgressBar',
	component: ProgressBarControl,
	tags: []
};

export const Simple = {
	args: {}
}; 

// Scenario from 0 to 100
export const progress = {
	args: { 
		modelValue: 50,
		showLimits: true,
	}
}; 

// Scenario with min, max and modelValue
export const simple = {
	args: {
		min: 10, 
		max: 30, 
		modelValue: 20,
		showLimits: true,
	}
}; 

// Scenario with text
export const text = {
	args: {
		min: 50, 
		modelValue: 75,
		text: 'Hi, I am a progress bar',
		showLimits: true,
	}
};

// Scenario with a light color 
export const darkColor = {
	args: {
		min: 10, 
		max: 30, 
		modelValue: 20,
		barColor: '#000000',
		showLimits: true,
	}
};

//Scenario with a dark color
export const lightColor = {
	args: {
		min: 10, 
		max: 30, 
		modelValue: 20,
		barColor: '#FFFFFF',
		showLimits: true,
	}
};

