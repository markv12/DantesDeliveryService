import { config as dotEnvConfig } from 'dotenv'
dotEnvConfig()
import * as c from './common'
import * as server from './server/index'

server.init()
