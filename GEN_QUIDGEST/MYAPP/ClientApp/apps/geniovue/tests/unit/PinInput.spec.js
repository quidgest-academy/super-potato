import '@testing-library/jest-dom'
import { fireEvent, render} from '@testing-library/vue'
import userEvent from "@testing-library/user-event";

import QPinInput from '@/components/rendering/QPinInput.vue';

describe('QPinInput', () => {

	it("renders correct number of input fields", async () => {
		const charNumber = 10;
		const wrapper = render(QPinInput, {
			props: {
				dataTestid: "pin-input-test1",
				charNumber: charNumber,
			},
		});
		const inputFields = wrapper.getAllByTestId('pin-input-test1');
		expect(inputFields.length).toBe(charNumber);
	});

	it('updates pin value when a digit is entered', async () => {
		const wrapper = render(QPinInput, {
			props: {
				dataTestid: "pin-input-test2",
				charNumber: 5,
			},
		});
		const inputFields = wrapper.getAllByTestId('pin-input-test2');
		await fireEvent.keyUp(inputFields[0], {key:'1'});
		expect(wrapper).toEmitModelValue("1");
	});
  
	it('does not insert a invalid digit in pin', async () => {
		const wrapper = render(QPinInput, {
			props: {
				dataTestid: "pin-input-test3",
				charNumber: 5,
			},
		});
		const inputFields = wrapper.getAllByTestId('pin-input-test3');
		await fireEvent.keyUp(inputFields[0], {key:'«'});
		expect(wrapper.emitted()["update:modelValue"]).toBeFalsy();
	});

	it('moves focus to the next input box when a digit is entered', async () => {
		const wrapper = render(QPinInput, {
			props: {
				dataTestid: "pin-input-test4",
				charNumber: 5,
			},
		});
		const inputFields = await wrapper.getAllByTestId('pin-input-test4');
    
		await fireEvent.keyUp(inputFields[0], {key:'1'});

		expect(wrapper).toEmitModelValue("1");
		expect(inputFields[1]).toHaveFocus();
	});

	it('clears the input box and moves focus left when Backspace is pressed', async () => {
		const wrapper = render(QPinInput, {
			props: {
				dataTestid: "pin-input-test5",
				charNumber: 5,
			},
		});
		const inputFields = await wrapper.getAllByTestId('pin-input-test5');
    
		await fireEvent.keyUp(inputFields[0], {key:'1'});
		expect(wrapper).toEmitModelValue("1");
		await fireEvent.keyUp(inputFields[1], {key:'1'});
		expect(wrapper).toEmitModelValue("11");
		await fireEvent.keyUp(inputFields[1], {key:'Backspace'});
		expect(wrapper).toEmitModelValue("1");
		expect(inputFields[0]).toHaveFocus();
	});

	it ('changes positions correctly', async () => {
		const wrapper = render(QPinInput, {
			props: {
				dataTestid: "pin-input-test6",
				charNumber: 5,
			},
		});
		const inputFields = await wrapper.getAllByTestId('pin-input-test6');
    
		await userEvent.type(inputFields[0], 'ArrowRight');
		expect(inputFields[0]).toHaveFocus();
		await userEvent.type(inputFields[0], '1');
		expect(inputFields[1]).toHaveFocus();
		await userEvent.keyboard('{ArrowLeft}');
		expect(inputFields[0]).toHaveFocus();
		await userEvent.keyboard('{ArrowRight}');
		expect(inputFields[1]).toHaveFocus();
	});

	it('clears the input box and stays in the same position when Delete is pressed', async () => {
		const wrapper = render(QPinInput, {
			props: {
				dataTestid: "pin-input-test7",
				charNumber: 5,
			},
		});
		const inputFields = await wrapper.getAllByTestId('pin-input-test7');
    
		await userEvent.type(inputFields[0], '1');
		expect(wrapper).toEmitModelValue("1");
		await userEvent.type(inputFields[1], '1');
		expect(wrapper).toEmitModelValue("11");
		await userEvent.keyboard('{ArrowLeft}');
		await userEvent.keyboard('{Delete}');
		expect(wrapper).toEmitModelValue("1");
		expect(inputFields[1]).toHaveFocus();
	});
});