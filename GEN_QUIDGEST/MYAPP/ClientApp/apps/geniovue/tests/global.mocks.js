import { vi } from 'vitest'

// Mock the global fetch function
const mockFetchResponse = {
	ok: true,
	status: 200,
	json: () => Promise.resolve({ success: true, message: 'Mock data from fetch' }),
	text: () => Promise.resolve('Mock text data'),
	blob: () => Promise.resolve(new Blob())
}

vi.stubGlobal(
	'fetch',
	vi.fn(() => {
		return Promise.resolve(mockFetchResponse)
	})
)
