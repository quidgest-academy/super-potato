export default {
	props: {
		/**
		 * The help object.
		 */
		helpControl: {
			type: Object
		}
	},

	computed: {
		tooltipText() {
			if(!this.helpControl) return ""
			if(this.helpControl.shortHelp.type === 'Tooltip' && this.helpControl.shortHelp.text)
				return this.helpControl.shortHelp.text
			return ""
		},
		popoverText() {
			if(!this.helpControl) return ""
			if(this.helpControl.shortHelp.type === 'Popover' && this.helpControl.shortHelp.text)
			{
				return this.helpControl.shortHelp.text
			}
			else if (this.helpControl.detailedHelp !== undefined && this.helpControl.detailedHelp.type === 'Popover' && this.helpControl.detailedHelp.text) 
			{
				return this.helpControl.detailedHelp.text
			}
			return ""
		},
		subtitleText() {
			if(!this.helpControl) return ""
			if(this.helpControl.shortHelp.type === 'Subtitle' && this.helpControl.shortHelp.text)
			{
				return this.helpControl.shortHelp.text
			}
			else if (this.helpControl.detailedHelp !== undefined && this.helpControl.detailedHelp.type === 'Subtitle' && this.helpControl.detailedHelp.text) 
			{
				return this.helpControl.detailedHelp.text
			}
			return ""
		},

		hasInfoBanner() {
			if(!this.helpControl) return false
			return this.helpControl.shortHelp.type === 'Info-banner' || this.helpControl.detailedHelp?.type === 'Info-banner'
		},

		anchorId() {
			if (this.hasLabel) {
				return `#${this.labelId}`;
			}
			return this.controlId ? `#${this.controlId}` : "";
		},

		isMarkdown() {			
			if (this.helpControl?.detailedHelp?.markdown === true)
				return true
			else
				return false
		}
	}
}
