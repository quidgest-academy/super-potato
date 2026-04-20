import '@testing-library/jest-dom'
import { render } from './utils'
import userEvent from '@testing-library/user-event'

import QMask from '@/components/inputs/QMask.vue'

describe('QMask.vue', () => {
	// Region NC
	it('Format the string with the corresponding type NC', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'NC',
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, '1350182B')
		expect(smallInput).toHaveValue('1350182')
	})

	// Region CP
	it('Format the string with the corresponding type CP', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'CP',
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, '1256b18y6')
		expect(smallInput).toHaveValue('1256-186')
	})

	// Region IB
	it('Format the string with the corresponding type IB', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'IB',
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, '4123fasdf4124a12341234123b41')
		expect(smallInput).toHaveValue('4123 4124 12341234123 41')
	})

	// Region SS
	it('Format the string with the corresponding type SS', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'SS',
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, '123sdfv-as12343123')
		expect(smallInput).toHaveValue('12312343123')
	})

	// Region IN
	it('Format the string with the corresponding type IN', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'IN',
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, 'PT4400350651271\'\'AA9133742503')
		expect(smallInput).toHaveValue('PT44 0035 0651 2719 1337 4250 3')
	})

	// Region  MA
	it('Format the string with the corresponding type MA', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'MA',
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, 'as2\'1º-1aa')
		expect(smallInput).toHaveValue('AS-21-1A')
	})

	// Region EM
	it('Format the string with the corresponding type EM', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'EM',
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, 'email_1\'~´@@&/_@#|quidgest.com')
		expect(smallInput).toHaveValue('email_1@quidgest.com')
	})

	// Region MP
	it('Format the string with the corresponding type MP', async () => {
		const wrapper = render(QMask, {
			props: {
				maskType: 'MP',
				maskFormat: {
					mask: 'AA-AA',
					tokens: { A: { pattern: /[A-Za-z0-9]/ } }
				},
				dataTestid: 'maskInput'
			}
		})

		const smallInput = await wrapper.findByTestId('maskInput')
		await userEvent.type(smallInput, '~´~´1~´\'\'231')
		expect(smallInput).toHaveValue('12-31')
	})
})
