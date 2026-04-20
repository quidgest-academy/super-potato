import { postData } from '../../network'

async function getPlatform() {
	const { platform } = await navigator.userAgentData.getHighEntropyValues(['platform'])
	return platform
}

export default async function openQSign(controller, action, params, fnCallback) {
	let intervalId = null

	return new Promise((resolve) => {
		const protocol = window.location.protocol

		postData(
			controller,
			action,
			{
				...params,
				protocol
			},
			async (data) => {
				if (data.success === true) {
					const platform = await getPlatform(),
						appUrl = data.rec.replace('http:', protocol)

					if (platform.toLocaleLowerCase().indexOf('win') >= 0)
						window.location = 'myscheme:|' + appUrl
					else if (platform.toLocaleLowerCase().indexOf('mac') >= 0)
						window.location = 'qsign://' + appUrl
					else {
						resolve(false)
						return
					}

					/**
					 * The client-side application is locked, and an external app (ours) is running alongside on the client's machine.
					 * The client-side will make requests every x milliseconds (in the MVC version it is set to 100 ms)
					 *   to check if the external application has finished what it had to do.
					 * The setInterval cannot be replaced by the timeout because it will force the script to be invoked recursively and it will throw error when it reaches the maximum stack.
					 */
					intervalId = setInterval(() => {
						postData(
							'Home',
							'RefreshDbPDF',
							null,
							(d) => {
								if (d && d.success && !d.loading) {
									if (intervalId !== null) clearInterval(intervalId)

									resolve(true)
									if (fnCallback) fnCallback({ result: d, params })
								}
							},
							() => {
								if (intervalId !== null) clearInterval(intervalId)

								resolve(false)
							}
						)
					}, 750)
				} else {
					if (intervalId !== null) clearInterval(intervalId)

					resolve(false)
				}
			},
			() => {
				if (intervalId !== null) clearInterval(intervalId)

				resolve(false)
			}
		)
	})
}
