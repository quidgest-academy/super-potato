export default {
	simpleUsage() {
		return {
			infoMessages: [
				{
					id: 1,
					message: 'A non-dismissible information message.',
					type: 'info',
					isDismissible: false
				},
				{
					id: 2,
					message:
						'An informative banner. Nothing to do here, enjoy your day!',
					type: 'info',
					isDismissible: true
				},
				{
					id: 3,
					message: 'Changes made successfully.',
					type: 'success',
					isDismissible: true
				},
				{
					id: 4,
					message: 'There are invalid records!',
					type: 'warning',
					isDismissible: true
				},
				{
					id: 5,
					message: 'Keyboard not found. Press F1 to Resume.',
					type: 'error',
					isDismissible: true
				}
			]
		}
	}
}
