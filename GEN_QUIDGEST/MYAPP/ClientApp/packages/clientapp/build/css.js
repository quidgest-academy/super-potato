import { dirname, join, relative } from 'node:path'
import { fileURLToPath } from 'node:url'

import autoprefixer from 'autoprefixer'
import cssnano from 'cssnano'
import postcss from 'postcss'
import { compile, NodePackageImporter } from 'sass'

import { createFolder, readFile, throttleTasks, writeAsset } from './utils.js'

const __filename = fileURLToPath(import.meta.url)
const __dirname = dirname(__filename)
const srcPath = join(__dirname, '../src/')
const distPath = join(__dirname, '../dist/')

const nano = postcss([
	cssnano({
		preset: [
			'default',
			{
				mergeLonghand: false,
				convertValues: false,
				cssDeclarationSorter: false,
				reduceTransforms: false
			}
		]
	})
])

/**
 * Renders prefixed and minified precompiled CSS stylesheets.
 * @param {string} cssCode - Precompiled CSS code.
 * @returns {Promise<string>} A Promise that resolves with the minified CSS code.
 */
async function renderPrecompiledCss(cssCode) {
	const postcssResult = await postcss([autoprefixer]).process(cssCode, { from: void 0 })
	postcssResult.warnings().forEach((warn) => {
		// eslint-disable-next-line no-console
		console.warn(warn.toString())
	})

	const prefixedCss = postcssResult.css
	const minifiedCss = (await nano.process(prefixedCss, { from: void 0 })).css

	return Promise.all([
		writeAsset(`${distPath}index.css`, prefixedCss, true),
		writeAsset(`${distPath}index.min.css`, minifiedCss, true)
	])
}

/**
 * Extracts and cleans SCSS files,
 * removing comments and unnecessary newlines.
 * @param {string} path - path to the SCSS file.
 * @returns {Promise<string>} A Promise that resolves with the clean SCSS code.
 */
function getCssContent(path) {
	return new Promise((resolve) => {
		const code = readFile(path)

		// Clean up file
		code
			// remove comments (/* ... */ and // ...)
			.replace(/\/\*[\s\S]*?\*\/|\/\/.*/g, '')
			// collapse multiple newlines
			.replace(/[\r\n]+/g, '\r\n')

		resolve(code)
	})
}

/**
 * Computes the destination of a file on the dist directory, based on the original file path.
 * @param {string} path - path to the SCSS file.
 * @returns {string} The path of the file on the dist directory.
 */
function getDistLocation(path) {
	const filePath = fileURLToPath(path)
	const relativePath = relative(srcPath, filePath)

	return join(distPath, relativePath)
}

/**
 * Generates style assets from a given index SCSS file.
 * @param {string} index - Path to the index CSS file.
 * @returns {Promise<void>} A Promise that resolves when generation is complete.
 */
function generateStyleAssets(index) {
	const src = join(__dirname, '..', index)

	const result = compile(src, {
		silenceDeprecations: ['import'],
		// https://sass-lang.com/documentation/js-api/classes/nodepackageimporter/
		importers: [new NodePackageImporter()]
	})

	// remove @charset declaration -- breaks Vite usage
	const cssCode = result.css.toString().replace('@charset "UTF-8";', '')
	const imports = result.loadedUrls

	const assetGenerationTasks = imports.map((path) => () => {
		const destination = getDistLocation(path)
		createFolder(dirname(destination))
		getCssContent(path).then((code) => writeAsset(destination, code))
	})
	// limit promise concurrency to max. 10 files at a time
	const throttledGeneration = throttleTasks(assetGenerationTasks, 10)

	return Promise.all([renderPrecompiledCss(cssCode), throttledGeneration])
}

createFolder(distPath)
generateStyleAssets('src/styles/index.scss').catch((e) => {
	// eslint-disable-next-line no-console
	console.error(e)
	process.exit(1)
})
