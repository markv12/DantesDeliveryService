import fs from 'fs'
import path from 'path'
import * as c from '../common'

const dataDirectoryPath = path.join('./', 'data')
if (!fs.existsSync(dataDirectoryPath)) {
  fs.mkdirSync(dataDirectoryPath)
  c.log('Created ./data directory in server root')
}

import addScore from './addScore'

const db = {
  async addScore(
    ip?: string,
    location?: LocationData,
    score?: number,
  ): Promise<{
    region?: string
    country?: string
    regionRank: number
    countryRank: number
    worldRank: number
  }> {
    const ranks = {
      region: location?.regionName,
      country: location?.country,
      regionRank: 0,
      countryRank: 0,
      worldRank: 0,
    }
    if (!score || !ip) return ranks

    // add and get world ranking
    const worldRank = addScore('world', ip, score)
    const countryRank = addScore(
      `country/${location?.country}`,
      ip,
      score,
    )
    const regionRank = addScore(
      `region/${location?.country}-${location?.regionName}`,
      ip,
      score,
    )

    await Promise.all([worldRank, countryRank, regionRank])

    ranks.worldRank = await worldRank
    ranks.countryRank = await countryRank
    ranks.regionRank = await regionRank

    return ranks
  },

  async wipe(): Promise<void> {
    await fs.promises.rm(dataDirectoryPath, {
      recursive: true,
    })
    fs.mkdirSync(dataDirectoryPath)
  },

  async getCount(dbPath: string): Promise<number> {
    dbPath = dbPath.replace(/..\//g, '')

    if (!fs.existsSync(path.join('./', 'data', dbPath))) {
      return 0
    }

    const data =
      (await fs.promises.readFile(
        path.join('./', 'data', dbPath + '.json'),
        'utf8',
      )) || '[]'
    try {
      const parsedData = JSON.parse(data)
      return parsedData.length
    } catch (err) {
      c.log('red', err)
      return 0
    }
  },

  async get(dbPath: string): Promise<any[]> {
    dbPath = dbPath.replace(/..\//g, '')

    if (!fs.existsSync(path.join('./', 'data', dbPath))) {
      return []
    }

    const data =
      (await fs.promises.readFile(
        path.join('./', 'data', dbPath + '.json'),
        'utf8',
      )) || '[]'
    try {
      const parsedData = JSON.parse(data)
      return parsedData
    } catch (err) {
      c.log('red', err)
      return []
    }
  },
}

export default db
