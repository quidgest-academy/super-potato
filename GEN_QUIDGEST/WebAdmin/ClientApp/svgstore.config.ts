import { readdirSync, readFileSync, writeFileSync } from 'node:fs'
import { dirname, extname, join, parse } from 'node:path'
import { fileURLToPath } from 'node:url'

// @ts-expect-error svgstore does not export types
import svgstore from 'svgstore'

/**
 * Bundles all SVG icons in a directory into a single sprite sheet.
 */
function bundleSvgIcons(sourceDir: string, outputFile: string) {
	const files = readdirSync(sourceDir)
	const sprites = svgstore()

	files.forEach((file) => {
		if (extname(file) === '.svg') {
			const id = parse(file).name
			const content = readFileSync(join(sourceDir, file), 'utf8')
			sprites.add(id, content)
		}
	})

	writeFileSync(outputFile, sprites.toString())
}

/**
 * Project-specific bundler that generates the final SVG sprite file.
 */
export function generateSvgSpriteBundle() {
	const __filename = fileURLToPath(import.meta.url)
	const __dirname = dirname(__filename)

	bundleSvgIcons(
		join(__dirname, './public/Content/svg/'),
		join(__dirname, './public/Content/svgbundle.svg')
	)

	console.log(
	'\x1b[36m%s\x1b[0m',
		`✅ SVG sprite bundle generated successfully at: ./public/Content/svgbundle.svg`
	)
}
