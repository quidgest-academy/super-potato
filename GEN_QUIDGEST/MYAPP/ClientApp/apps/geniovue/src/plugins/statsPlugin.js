import fs from 'fs'
import json from 'big-json'
import path from 'path'

class StatsPlugin
{
	constructor(options)
	{
		this.showFullStats = (options || {}).showFullStats === true
	}

	apply(compiler)
	{
		if (this.showFullStats)
		{
			compiler.hooks.done.tapPromise('q-stats-plugin', (stats) => {
				// eslint-disable-next-line no-console
				console.log(`Done - ${stats.endTime - stats.startTime} ms`)

				const statsJson = stats.toJson()
				return this.saveStats(statsJson)
			})
		}
		else
		{
			compiler.hooks.done.tap('q-stats-plugin', (stats) => {
				// eslint-disable-next-line no-console
				console.log(`Done - ${stats.endTime - stats.startTime} ms`)
			})
		}
	}

	saveStats(data)
	{
		return new Promise((resolve) => {
			const profilingFolder = path.resolve('./profiling')

			if (!fs.existsSync(profilingFolder))
				fs.mkdirSync(profilingFolder)

			const writeStream = fs.createWriteStream(path.resolve(`./profiling/stats_${Date.now()}.json`), { flag: 'w+' })
			json.createStringifyStream({ body: data }).pipe(writeStream)
			writeStream.on('close', () => {
				resolve()
			})
		})
	}
}

export default StatsPlugin
