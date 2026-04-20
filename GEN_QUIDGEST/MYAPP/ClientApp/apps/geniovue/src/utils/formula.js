import isEmpty from 'lodash-es/isEmpty'

/**
 * Runs the specified field formula.
 * @param {object} formula The formula
 * @param {object} context The context from where the formula should be invoked
 * @param {any} params The parameters to pass when calling the formula
 * @returns A promise with the result of the formula call.
 */
export async function validateFormula(formula, context, params)
{
	if (isEmpty(formula) || typeof formula.fnFormula !== 'function')
		throw new Error('The specified formula is empty.')
	if (typeof context !== 'object' || context === null)
		throw new Error('The formula context must be provided.')
	return await formula.fnFormula.call(context, params)
}

export default {
	validateFormula
}
