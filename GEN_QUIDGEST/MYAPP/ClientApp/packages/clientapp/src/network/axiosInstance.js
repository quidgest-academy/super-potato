import axios from 'axios'

import { handleDates } from './axiosResponseParsers'

export const axiosInstance = axios.create()

// Add a response interceptor
axiosInstance.interceptors.response.use((response) => {
	// To avoid overwriting if another interceptor already defined the same object (meta)
	response.config.meta = response.config.meta || {}
	// Set the End timestamp
	response.config.meta.requestEndAt = new Date().getTime()

	// Any status code that lie within the range of 2xx cause this function to trigger
	handleDates(response.data)

	return response
})

// Add a request interceptor
axiosInstance.interceptors.request.use((config) => {
	// Do something before request is sent
	// TODO: Convert datetime to ISO

	// To avoid overwriting if another interceptor already defined the same object (meta)
	config.meta = config.meta || {}
	// Set the Start timestamp
	config.meta.requestStartAt = new Date().getTime()

	return config
})

export default {
	axiosInstance
}
