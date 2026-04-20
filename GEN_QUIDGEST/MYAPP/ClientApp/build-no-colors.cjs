// eslint-disable-next-line @typescript-eslint/no-require-imports
const { spawn } = require('child_process')

// eslint-disable-next-line no-control-regex
const stripAnsi = (str) => str.replace(/\x1B\[[0-9;]*m/g, '')

const child = spawn('pnpm', ['build:packages', '&&', 'pnpm', 'build:app'], {
	stdio: 'pipe',
	shell: true
})

child.stdout.on('data', (data) => {
	process.stdout.write(stripAnsi(data.toString()))
})

child.stderr.on('data', (data) => {
	process.stderr.write(stripAnsi(data.toString()))
})

child.on('close', (code) => {
	process.exit(code)
})
