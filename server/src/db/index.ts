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
    regionRank: number
    countryRank: number
    worldRank: number
  }> {
    const ranks = {
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
      `region/${location?.regionName}`,
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
    await fs.promises.rmdir(dataDirectoryPath, {
      recursive: true,
    })
    fs.mkdirSync(dataDirectoryPath)
  },
}

export default db
