import * as c from '../../common'
import { Router } from 'express'
import { serverRunningSince } from '..'
import db from '../../db'

const routes = Router()

routes.use((req, res, next) => {
  if (req.query['pw'] !== process.env.PW) {
    res.status(401).end()
    return
  }
  c.log('gray', `Admin ${req.method} ${req.path}`)
  next()
})

routes.get('/wipedb', async (req, res) => {
  await db.wipe()
  res.json({ ok: true })
})

routes.get('/stats', async (req, res) => {
  res.json({
    serverRunningSince,
  })
})

export default routes
