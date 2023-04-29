import { config as dotEnvConfig } from 'dotenv'
dotEnvConfig()
import * as c from './common'
import * as server from './server/index'
import db from './db'
c.log(db)

server.init()
