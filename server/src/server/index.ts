import * as c from '../common'
import express from 'express'
import bodyParser from 'body-parser'
import cors from 'cors'
import helmet from 'helmet'

import scoresRoutes from './routes/score'
import adminRoutes from './routes/admin'

export let serverRunningSince

const appBase = `/ld53`

declare global {
  namespace Express {
    export interface Request {
      parsedIp?: string
      location?: LocationData
    }
  }
}

const app = express()
app.use((req, res, next) => {
  c.log('gray', `${req.method} ${req.path}`)
  next()
})
app.use(cors())
app.use(
  helmet({
    crossOriginResourcePolicy: {
      policy: 'cross-origin',
    },
    crossOriginOpenerPolicy: { policy: 'unsafe-none' },
    contentSecurityPolicy: false,
  }),
)
app.use(bodyParser.json())
app.use(bodyParser.urlencoded({ extended: true }))

app.get(appBase + '/', (req, res) => {
  res.json({ ok: true, serverRunningSince })
})
app.get(appBase + '/api', (req, res) => {
  res.json({ ok: true, serverRunningSince })
})

const apiPrefix = appBase + `/api`
app.use(apiPrefix + '/admin', adminRoutes)
app.use(apiPrefix + '/score', scoresRoutes)

export function init() {
  serverRunningSince = new Date()
  const port = parseInt(process.env.PORT || '') || 5053
  app.listen(port, '127.0.0.1', () => {
    c.log(`Server is running on 127.0.0.1:${port}`)
  })
}
