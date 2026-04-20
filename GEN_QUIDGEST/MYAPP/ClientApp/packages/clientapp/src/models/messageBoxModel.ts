// Types
import type {
	Icon,
	DialogButton,
	QDialogHandlers,
	QDialogOptions,
	QDialogProps,
	QButtonVariant
} from '@quidgest/ui/components'

/**
 * Supported icon types for MessageBox dialogs
 */
export type MessageBoxIconType = 'info' | 'success' | 'warning' | 'question' | 'error'

/**
 * Configuration for a MessageBox button
 */
export interface MessageBoxButton {
	/** The text label displayed on the button */
	label?: string
	/** The icon displayed on the button */
	icon?: Icon
	/** Callback function executed when the button is clicked */
	action?: (value?: string) => void
}

/**
 * Standard button configuration for MessageBox dialogs
 */
export interface MessageBoxButtons {
	/** Primary confirmation button (typically "OK", "Yes", "Save", etc.) */
	confirm?: MessageBoxButton
	/** Secondary cancel button (typically "Cancel", "No", etc.) */
	cancel?: MessageBoxButton
	/** Tertiary deny/reject button (typically "Don't Save", "Discard", etc.) */
	deny?: MessageBoxButton
}

/**
 * Internal icon configuration mapping for predefined icon types
 */
interface IconDataConfig {
	/** The actual icon identifier to use */
	icon: string
	/** The color theme for the icon */
	color: string
}

/**
 * A utility class for creating standardized message dialog boxes with customizable
 * buttons, icons, and behavior. Provides a consistent interface for displaying
 * information, confirmations, warnings, and error messages to users.
 *
 * @example
 * ```typescript
 * // Simple info message
 * const infoBox = new MessageBox('Operation completed successfully', 'success', 'Success');
 *
 * // Confirmation dialog with custom buttons
 * const confirmBox = new MessageBox(
 *   'Are you sure you want to delete this item?',
 *   'question',
 *   'Confirm Delete',
 *   {
 *     confirm: { label: 'Delete', action: () => deleteItem() },
 *     cancel: { label: 'Cancel', action: () => console.log('Cancelled') }
 *   }
 * );
 * ```
 */
export default class MessageBox {
	/** The main message text to display in the dialog */
	public readonly text: string

	/** The title shown in the dialog header */
	public readonly title: string

	/** The configured icon with styling information */
	public readonly icon: Icon

	/** Array of dialog buttons with their configurations */
	public readonly buttons: DialogButton[]

	/** Additional dialog configuration options */
	public readonly options: QDialogOptions

	/** Event handlers */
	public readonly handlers: QDialogHandlers

	/**
	 * Predefined icon configurations for standard message types
	 */
	private static readonly ICON_DATA_MAP: Record<MessageBoxIconType, IconDataConfig> = {
		info: {
			icon: 'information',
			color: 'info'
		},
		success: {
			icon: 'success',
			color: 'success'
		},
		warning: {
			icon: 'warning',
			color: 'warning'
		},
		question: {
			icon: 'question',
			color: 'info'
		},
		error: {
			icon: 'exclamation-sign',
			color: 'danger'
		}
	}

	/**
	 * Default dialog options applied when none are specified
	 */
	private static readonly DEFAULT_OPTIONS: Partial<QDialogOptions> = {
		size: 'small',
		dismissible: true,
		input: undefined
	}

	/**
	 * Creates a new MessageBox instance with the specified configuration.
	 *
	 * @param text - The main message text to display
	 * @param icon - The icon type or custom icon identifier (defaults to 'info')
	 * @param title - The dialog title (defaults to empty string)
	 * @param buttons - Configuration for dialog buttons (defaults to single OK button)
	 * @param options - Additional dialog options (optional)
	 * @param handlers Event handlers (optional)
	 *
	 * @throws {Error} When required parameters are missing or invalid
	 */
	constructor(
		text: string,
		icon: MessageBoxIconType | string = 'info',
		title: string = '',
		buttons: MessageBoxButtons = {},
		options: QDialogOptions = {},
		handlers: QDialogHandlers = {}
	) {
		// Validate required parameters
		if (!text || typeof text !== 'string') {
			throw new Error('MessageBox requires a valid text parameter')
		}

		this.text = text
		this.title = title

		// Merge provided options with defaults
		this.options = {
			...MessageBox.DEFAULT_OPTIONS,
			...options
		}

		// Configure icon based on type or use custom icon
		this.icon = this.configureIcon(icon)

		// Convert and validate buttons
		this.buttons = this.configureButtons(buttons)

		// Set event handlers
		this.handlers = handlers
	}

	/**
	 * Configures the icon based on the provided icon type or custom identifier
	 *
	 * @param iconInput - Either a predefined icon type or custom icon string
	 * @returns Configured Icon object
	 */
	private configureIcon(iconInput: MessageBoxIconType | string): Icon {
		const iconConfig = MessageBox.ICON_DATA_MAP[iconInput as MessageBoxIconType]

		return {
			icon: iconConfig?.icon ?? iconInput,
			type: 'svg',
			size: 12,
			color: iconConfig?.color ?? 'primary'
		}
	}

	/**
	 * Converts MessageBoxButtons configuration to DialogButton array format
	 *
	 * @param buttonsConfig - The button configuration object
	 * @returns Array of configured DialogButton objects
	 */
	private configureButtons(buttonsConfig: MessageBoxButtons): DialogButton[] {
		const buttonsArray: DialogButton[] = []

		// Process each button type
		for (const [buttonType, buttonConfig] of Object.entries(buttonsConfig)) {
			if (buttonConfig) {
				buttonsArray.push(this.createDialogButton(buttonType, buttonConfig))
			}
		}

		// Add default OK button if no buttons are configured
		if (buttonsArray.length === 0) {
			buttonsArray.push(this.createDefaultOkButton())
		}

		return buttonsArray
	}

	/**
	 * Creates a DialogButton from MessageBoxButton configuration
	 *
	 * @param buttonType - The type of button (confirm, cancel, deny)
	 * @param buttonConfig - The button configuration
	 * @returns Configured DialogButton
	 */
	private createDialogButton(buttonType: string, buttonConfig: MessageBoxButton): DialogButton {
		return {
			id: `dialog-button-${buttonType}`,
			icon: buttonConfig.icon || this.getDefaultButtonIcon(buttonType),
			action: buttonConfig.action,
			props: {
				label: buttonConfig.label || this.getDefaultButtonLabel(buttonType),
				variant: buttonType === 'confirm' ? ('bold' as QButtonVariant) : undefined
			}
		}
	}

	/**
	 * Creates the default OK button when no buttons are specified
	 *
	 * @returns Default OK DialogButton
	 */
	private createDefaultOkButton(): DialogButton {
		return {
			id: 'dialog-button-ok',
			action: undefined,
			props: {
				label: 'OK',
				variant: 'bold' as QButtonVariant
			}
		}
	}

	/**
	 * Gets the default label for a button type
	 *
	 * @param buttonType - The button type
	 * @returns Default label string
	 */
	private getDefaultButtonLabel(buttonType: string): string {
		const defaultLabels: Record<string, string> = {
			confirm: 'OK',
			cancel: 'Cancel',
			deny: 'No'
		}
		return defaultLabels[buttonType] || 'OK'
	}

	/**
	 * Gets the default icon for a button type
	 *
	 * @param buttonType - The button type
	 * @returns Default icon string
	 */
	private getDefaultButtonIcon(buttonType: string): Icon | undefined {
		const defaultIcons: Record<string, Icon> = {
			confirm: { icon: 'ok' } as Icon,
			cancel: { icon: 'cancel' } as Icon,
			deny: { icon: 'close' } as Icon
		}
		return defaultIcons[buttonType] || undefined
	}

	/**
	 * Gets the properties object to pass to the dialog component
	 *
	 * @returns Props object for dialog component
	 */
	public get props(): QDialogProps {
		return {
			title: this.title,
			text: this.text,
			icon: this.icon,
			buttons: this.buttons,
			size: this.options.size,
			dismissible: this.options.dismissible,
			input: this.options.input,
			class: 'q-dialog-message'
		}
	}

	/**
	 * Static factory method for creating info message boxes
	 *
	 * @param text - The message text
	 * @param title - Optional title (defaults to 'Information')
	 * @returns New MessageBox instance
	 */
	public static info(text: string, title: string = 'Information'): MessageBox {
		return new MessageBox(text, 'info', title)
	}

	/**
	 * Static factory method for creating success message boxes
	 *
	 * @param text - The message text
	 * @param title - Optional title (defaults to 'Success')
	 * @returns New MessageBox instance
	 */
	public static success(text: string, title: string = 'Success'): MessageBox {
		return new MessageBox(text, 'success', title)
	}

	/**
	 * Static factory method for creating warning message boxes
	 *
	 * @param text - The message text
	 * @param title - Optional title (defaults to 'Warning')
	 * @returns New MessageBox instance
	 */
	public static warning(text: string, title: string = 'Warning'): MessageBox {
		return new MessageBox(text, 'warning', title)
	}

	/**
	 * Static factory method for creating error message boxes
	 *
	 * @param text - The message text
	 * @param title - Optional title (defaults to 'Error')
	 * @returns New MessageBox instance
	 */
	public static error(text: string, title: string = 'Error'): MessageBox {
		return new MessageBox(text, 'error', title)
	}

	/**
	 * Static factory method for creating confirmation dialog boxes
	 *
	 * @param text - The confirmation message
	 * @param onConfirm - Callback for confirm action
	 * @param onCancel - Optional callback for cancel action
	 * @param title - Optional title (defaults to 'Confirm')
	 * @returns New MessageBox instance
	 */
	public static confirm(
		text: string,
		onConfirm: () => void,
		onCancel?: () => void,
		title: string = 'Confirm'
	): MessageBox {
		return new MessageBox(text, 'question', title, {
			confirm: { label: 'Yes', action: onConfirm },
			cancel: { label: 'No', action: onCancel }
		})
	}
}
