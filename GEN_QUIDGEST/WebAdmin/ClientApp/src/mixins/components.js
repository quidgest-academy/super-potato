// All components
import text_input from '@/components/Text_input.vue'
import numeric_input from '@/components/Numeric_input.vue'
import password_input from '@/components/Password_input.vue'
import static_text from '@/components/Static_text.vue'
import datetime_picker from '@/components/DateTime_input.vue'
import row from '@/components/Row.vue'
import qtable from '@/components/Table/QTable.vue'
import progress_bar from '@/components/ProgressBar.vue'
import image_input from '@/components/Image_input.vue'
import DataSystemBadge from '@/components/DataSystemBadge.vue'
import GroupBoxContainer from '@/components/GroupBoxContainer.vue'
import TabContainer from '@/components/TabContainer.vue'
import BaseInputStructure from '@/components/BaseInputStructure.vue'
import QControlWrapper from '@/components/ControlWrapper.vue'
import QRowContainer from '@/components/RowContainer.vue'
import QAlert from '@/components/QAlert.vue'
import QListEditor from '@/components/ListEditor.vue'

// Quidgest UI
import {
	QAccordion,
	QBadge,
	QButton,
	QButtonGroup,
	QButtonToggle,
	QCard,
	QCheckbox,
	QCollapsible,
	QDialog,
	QDropdownMenu,
	QField,
	QIcon,
	QIconSvg,
	QInputGroup,
	QLabel,
	QLineLoader,
	QOverlay,
	QPopover,
	QRadioButton,
	QRadioGroup,
	QSelect,
	QSpinnerLoader,
	QTextArea,
	QTextField,
	QTooltip,
	QPasswordField
} from '@quidgest/ui/components'

export default function ComponentsInit(app) {
	// Inputs
	app.component('TextInput', text_input)
	app.component('NumericInput', numeric_input)
	app.component('PasswordInput', password_input)
	app.component('DatetimePicker', datetime_picker)
	app.component('ImageInput', image_input)
	app.component('QListEditor', QListEditor)

	// Containers
	app.component('QGroupBoxContainer', GroupBoxContainer)
	app.component('QTabContainer', TabContainer)
	app.component('BaseInputStructure', BaseInputStructure)
	app.component('QControlWrapper', QControlWrapper)
	app.component('QRowContainer', QRowContainer)

	// Table
	// eslint-disable-next-line vue/multi-word-component-names
	app.component('Row', row) // TODO: Change all <row> to <q-control-row> or similar
	// eslint-disable-next-line vue/multi-word-component-names
	app.component('Qtable', qtable) // TODO: Change all <qtable> to <q-table

	// Misc
	app.component('DataSystemBadge', DataSystemBadge)
	app.component('QAlert', QAlert)
	app.component('ProgressBar', progress_bar)
	app.component('StaticText', static_text)

	// QuidgestUI
	app.component('QAccordion', QAccordion)
	app.component('QBadge', QBadge)
	app.component('QButton', QButton)
	app.component('QButtonGroup', QButtonGroup)
	app.component('QButtonToggle', QButtonToggle)
	app.component('QCard', QCard)
	app.component('QCheckbox', QCheckbox)
	app.component('QCollapsible', QCollapsible)
	app.component('QDialog', QDialog)
	app.component('QDropdownMenu', QDropdownMenu)
	app.component('QField', QField)
	app.component('QIcon', QIcon)
	app.component('QIconSvg', QIconSvg)
	app.component('QInputGroup', QInputGroup)
	app.component('QLabel', QLabel)
	app.component('QLineLoader', QLineLoader)
	app.component('QOverlay', QOverlay)
	app.component('QPopover', QPopover)
	app.component('QRadioButton', QRadioButton)
	app.component('QRadioGroup', QRadioGroup)
	app.component('QSelect', QSelect)
	app.component('QSpinnerLoader', QSpinnerLoader)
	app.component('QTextArea', QTextArea)
	app.component('QTextField', QTextField)
	app.component('QTooltip', QTooltip)
	app.component('QPasswordField', QPasswordField)
}
