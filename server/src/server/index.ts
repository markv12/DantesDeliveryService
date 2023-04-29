import * as c from '../common'
import express from 'express'
import bodyParser from 'body-parser'
import cors from 'cors'
import helmet from 'helmet'

import getRoutes from './routes/get'
import adminRoutes from './routes/admin'

export let serverRunningSince

const app = express()
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

app.use(async (req, res, next) => {
  const ip = req.headers['x-forwarded-for'] || req.ip
  const location = await c.getLocationFromIp(
    Array.isArray(ip) ? ip[0] : ip,
  )
  c.log('gray', `${req.method} ${req.path} ${ip}`, location)
  next()
})

const apiPrefix = `/api`
app.use(apiPrefix + '/get', getRoutes)
app.use(apiPrefix + '/admin', adminRoutes)

export function init() {
  serverRunningSince = new Date()
  const port = process.env.PORT || 3053
  app.listen(port, () => {
    c.log(`Server is running on port ${port}`)
  })
}
