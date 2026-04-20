import { useGenericDataStore, useGenericLayoutDataStore } from '@quidgest/clientapp/stores'
import { focusElement } from '@quidgest/clientapp/utils/genericFunctions'

/**
 * Removes the specified modal/popup, or the last one if no id is passed.
 * @param {string} modalId The id of the modal
 */

export function removeModal(modalId) {
	// Remove modal popup
	const genericDataStore = useGenericDataStore()
	const removedModal = genericDataStore.removeModal(modalId)

	// Focus on the element / control that opened the popup
	// Using setTimeout with 0 to wait until the call stack is clear
	// so the focus doesn't happen before the modal is fully removed
	// because the focus wrap can still be active
	// which prevents focusing on the return element
	setTimeout(() => {
		focusElement(removedModal?.returnElement)
	}, 0)
}

/**
 * Sets specific progress bar properties.
 *
 * Configuration structure:
 *   {number} props.progress - The percentage of progress.
 *   {string} props.text - The displayed text.
 *   {boolean} props.striped - Whether the progress bar should be striped.
 *   {boolean} props.animated - Whether the progress bar should be animated.
 *   {boolean} props.mini - Whether the progress bar should be minimal.
 *   {string} modalProps.title - The displayed title.
 *   {array} modalProps.buttons - A list of buttons to be made available.
 *
 * @param {object} modalProps Configuration of the progress bar container
 * @param {object} props Progress bar configuration
 * @param {object} handlers Progress bar event handlers
 */
export function setProgressBar(modalProps, props, handlers) {
	const layoutDataStore = useGenericLayoutDataStore()
	layoutDataStore.setProgressBar(modalProps, props, handlers)
}

/**
 * Resets the progress bar to default values.
 * Mostly it will be used to close the progress bar.
 */
export function resetProgressBar() {
	const layoutDataStore = useGenericLayoutDataStore()
	layoutDataStore.resetProgressBar()
}

/**
 * Sets whether the cookies are visible.
 * @param {boolean} val The value of the cookies visibility
 */
export function setShowCookies(val)
{
	const genericDataStore = useGenericDataStore()
	genericDataStore.setShowCookies(val)
}
