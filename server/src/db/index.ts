import fs from 'fs'
import path from 'path'
import * as c from '../common'

const dataDirectoryPath = path.join('./', 'data')
if (!fs.existsSync(dataDirectoryPath)) {
  fs.mkdirSync(dataDirectoryPath)
  c.log('Created ./data directory in server root')
}

const db = {}

export default db
