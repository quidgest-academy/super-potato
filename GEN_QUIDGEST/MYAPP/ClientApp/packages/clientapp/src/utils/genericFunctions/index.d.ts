export function displayMessage(
	text: string,
	icon?: string,
	title?: string,
	buttons?: object,
	options?: object
): void

export function validateFileExtAndSize(file: File, extensions?: string[], maxSize?: number): number

export function isEmpty(value: string | number | Date | boolean | undefined): boolean

export function validateTexts(
	requiredTexts: Record<string, string>,
	texts: Record<string, string>
): boolean
