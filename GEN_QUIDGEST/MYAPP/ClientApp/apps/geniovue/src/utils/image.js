import { forceDownload } from '@quidgest/clientapp/network'

/**
 * Returns the URL of the image.
 * @param {String|Object} image - The ID of the image.
 * @returns A base64 representation of the provided image.
 */
export function getImageURL(image)
{
	// Here we are dealing with various cases:
	// - If image is not provided returns an empty path
	// - If image is string use it directly
	// - If image is dataURL object, create image url using data, encoding and data format
	return image
		? typeof image === 'object'
			? `data:image/${image.dataFormat};${image.encoding},${image.data}`
			: image
		: ''
}

/**
 * Returns the MIME type of the image.
 * @param {String|Object} image - The image.
 * @returns The MIME type of the provided image.
 */
export function getImageType(image)
{
	const base64Img = getImageURL(image)

	if (base64Img?.length > 0)
		return base64Img.split(';')[0]?.split('/').pop()
	return undefined
}

/**
 * Handles the downloading of the image.
 * @param {String|Object} image - The image.
 */
export function downloadImage(image)
{
	const imgData = getImageURL(image)
	const imgType = getImageType(image)
	const fileName = 'Image' // TODO: Use the original name of the file (to be passed as parameter)

	forceDownload(imgData, fileName, imgType, false, false)
}
