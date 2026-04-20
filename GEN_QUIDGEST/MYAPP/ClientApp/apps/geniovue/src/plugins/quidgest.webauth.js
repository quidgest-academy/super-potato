import netAPI from '@quidgest/clientapp/network'
import { displayMessage } from '@quidgest/clientapp/utils/genericFunctions'

// From: https://github.com/abergs/fido2-net-lib

//************** mfa.register ******************
export async function createRegisterWebAuth()
{
	netAPI.postData('Home', 'WebAuthn2FAMakeCredentialOptions', null, async (dataWebAuth) => {
		dataWebAuth.options = JSON.parse(dataWebAuth.options)

		if (dataWebAuth.options.status !== 'ok')
		{
			// eslint-disable-next-line no-console
			console.error('Error creating credential options.')
			return
		}

		// Turn the challenge back into the accepted format of padded base64
		dataWebAuth.options.challenge = coerceToArrayBuffer(dataWebAuth.options.challenge)
		// Turn ID into a UInt8Array Buffer for some reason
		dataWebAuth.options.user.id = coerceToArrayBuffer(dataWebAuth.options.user.id)

		dataWebAuth.options.excludeCredentials = dataWebAuth.options.excludeCredentials.map((c) => {
			c.id = coerceToArrayBuffer(c.id)
			return c
		})

		if (dataWebAuth.options.authenticatorSelection.authenticatorAttachment === null)
			dataWebAuth.options.authenticatorSelection.authenticatorAttachment = undefined

		let newCredential
		try
		{
			newCredential = await navigator.credentials.create({
				publicKey: dataWebAuth.options
			})
		}
		catch
		{
			const msg =
				'Could not create credentials in browser. Probably because the username is already registered with your authenticator. Please change username or authenticator.'
			displayMessage(msg, 'error')
		}

		try
		{
			registerNewCredential(newCredential)
		}
		catch (e)
		{
			displayMessage(e.message ? e.message : e, 'error')
		}
	})
}

// This should be used to verify the auth data with the server
export async function registerNewCredential(newCredential)
{
	// Move data into Arrays incase it is super long
	const attestationObject = new Uint8Array(newCredential.response.attestationObject)
	const clientDataJSON = new Uint8Array(newCredential.response.clientDataJSON)
	const rawId = new Uint8Array(newCredential.rawId)

	const data = {
		id: newCredential.id,
		rawId: coerceToBase64Url(rawId),
		type: newCredential.type,
		extensions: newCredential.getClientExtensionResults(),
		response: {
			AttestationObject: coerceToBase64Url(attestationObject),
			clientDataJson: coerceToBase64Url(clientDataJSON)
		}
	}

	netAPI.postData('Home', 'WebAuthn2FAMakeCredentialOptions2', data, (response) => {
		// show error
		if (!response.Success)
		{
			displayMessage('Error creating credential', 'error')
			return
		}

		// show success
		displayMessage('You\'ve registered successfully.', 'success', 'Registration Successful!', null, { timeout: 2000 })
	})
}

//************** mfa.login ******************
export async function handleSignInWebAuth(returnData)
{
	// send to server for registering
	netAPI.postData(
		'Account',
		'WebAuthn2FAAssertionOptions',
		null,
		async (response) => {
			const makeAssertionOptions = JSON.parse(response.options)

			// show options error to user
			if (makeAssertionOptions.status !== 'ok')
			{
				displayMessage(makeAssertionOptions.errorMessage, 'error')
				return
			}

			// todo: switch this to coercebase64
			const challenge = makeAssertionOptions.challenge.replace(/-/g, '+').replace(/_/g, '/')
			makeAssertionOptions.challenge = Uint8Array.from(atob(challenge), (c) => c.charCodeAt(0))

			// fix escaping. Change this to coerce
			makeAssertionOptions.allowCredentials.forEach((listItem) => {
				const fixedId = listItem.id.replace(/_/g, '/').replace(/-/g, '+')
				listItem.id = Uint8Array.from(atob(fixedId), (c) => c.charCodeAt(0))
			})

			// ask browser for credentials (browser will ask connected authenticators)
			let credential
			try
			{
				credential = await navigator.credentials.get({ publicKey: makeAssertionOptions })
			}
			catch (err)
			{
				displayMessage(err.message ? err.message : err, 'error')
			}

			try
			{
				const authData = new Uint8Array(credential.response.authenticatorData)
				const clientDataJSON = new Uint8Array(credential.response.clientDataJSON)
				const rawId = new Uint8Array(credential.rawId)
				const sig = new Uint8Array(credential.response.signature)
				const data = {
					id: credential.id,
					rawId: coerceToBase64Url(rawId),
					type: credential.type,
					extensions: credential.getClientExtensionResults(),
					response: {
						authenticatorData: coerceToBase64Url(authData),
						clientDataJson: coerceToBase64Url(clientDataJSON),
						signature: coerceToBase64Url(sig)
					}
				}

				netAPI.postData(
					'Account',
					'WebAuthn2FAMakeAssertion',
					{
						data: JSON.stringify(data),
						returnUrl: returnData.Redirect
					},
					(response2) => {
						// eslint-disable-next-line no-console
						console.log(response2)
					}
				)
			}
			catch (e)
			{
				// eslint-disable-next-line no-console
				console.log('Could not verify assertion: ' + e)
			}
		},
		() => {
			displayMessage('Request to server failed', 'error')
		}
	)
}

//************** HELPERS **************
function coerceToArrayBuffer(thing, name)
{
	if (typeof thing === 'string')
	{
		// base64url to base64
		thing = thing.replace(/-/g, '+').replace(/_/g, '/')

		// base64 to Uint8Array
		const str = window.atob(thing)
		const bytes = new Uint8Array(str.length)
		for (let i = 0; i < str.length; i++)
			bytes[i] = str.charCodeAt(i)
		thing = bytes
	}

	// Array to Uint8Array
	if (Array.isArray(thing))
		thing = new Uint8Array(thing)

	// Uint8Array to ArrayBuffer
	if (thing instanceof Uint8Array)
		thing = thing.buffer

	// error if none of the above worked
	if (!(thing instanceof ArrayBuffer))
		throw new TypeError('Could not coerce \'' + name + '\' to ArrayBuffer')

	return thing
}

function coerceToBase64Url(thing)
{
	// Array or ArrayBuffer to Uint8Array
	if (Array.isArray(thing))
		thing = Uint8Array.from(thing)

	if (thing instanceof ArrayBuffer)
		thing = new Uint8Array(thing)

	// Uint8Array to base64
	if (thing instanceof Uint8Array)
	{
		let str = ''
		const len = thing.byteLength

		for (let i = 0; i < len; i++)
			str += String.fromCharCode(thing[i])
		thing = window.btoa(str)
	}

	if (typeof thing !== 'string')
		throw new Error('could not coerce to string')

	// base64 to base64url
	// NOTE: "=" at the end of challenge is optional, strip it off here
	thing = thing.replace(/\+/g, '-').replace(/\//g, '_').replace(/=*$/g, '')

	return thing
}

export default {
	createRegisterWebAuth,
	registerNewCredential,
	handleSignInWebAuth
}
