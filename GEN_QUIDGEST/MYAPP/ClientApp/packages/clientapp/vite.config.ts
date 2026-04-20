import { dirname, resolve, join } from 'node:path'
import { fileURLToPath } from 'node:url'
import { nodeResolve } from '@rollup/plugin-node-resolve'
import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vite'
import dts from 'vite-plugin-dts'
import pkg from './package.json'
import fg from 'fast-glob'

const __filename = fileURLToPath(import.meta.url)
const __dirname = dirname(__filename)
const src = resolve(__dirname, 'src')

// Find all index.ts files under src
function getEntryPoints() {
	const entries: Record<string, string> = {}
	const indexFiles = fg.sync(['**/index.{ts,js}'], { cwd: src })

	indexFiles.forEach((file) => {
		const filePath = join(src, file)
		// Create entry name from file path, preserving directory structure
		const entryPath = dirname(file) === '.' ? '' : dirname(file)
		const entryName = entryPath ? `${entryPath}/index` : 'index'

		entries[entryName] = resolve(filePath)
	})

	return entries
}

const entryPoints = getEntryPoints()

// https://vitejs.dev/config/
export default defineConfig({
	resolve: {
		alias: [
			/*
			 * We recommend to not use aliases in the lib's source,
			 * because they will leak into the generated d.ts files and then
			 * break the lib's types in the consuming app.
			 */
		]
	},
	build: {
		lib: {
			entry: entryPoints,
			name: 'Quidgest Vue Client App',
			formats: ['es']
		},
		emptyOutDir: false,
		rollupOptions: {
			external: Object.keys(pkg.peerDependencies),
			output: {
				dir: 'dist',
				preserveModules: true,
				preserveModulesRoot: src,
				entryFileNames: ({ name }) => {
					// Vendor files
					if (name.includes('node_modules')) {
						const parts = name.split('node_modules/')
						const lastPart = parts[parts.length - 1]
						const [pkg, ...pathParts] = lastPart.split('/')
						return `vendors/${pkg}/${pathParts.join('/')}.js`
					}

					// For entry points, maintain the directory structure
					return `${name}.js`
				}
			}
		}
	},
	plugins: [
		dts({
			outDir: 'dist',
			tsconfigPath: './tsconfig.build.json',
			cleanVueFileName: true
		}),
		nodeResolve(),
		vue()
	]
})
