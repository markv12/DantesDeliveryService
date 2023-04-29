import * as c from '../../common'
import { Router } from 'express'
import db from '../../db'

const routes = Router()

routes.use(async (req, res, next) => {
  const ip = req.headers['x-forwarded-for'] || req.ip
  req.ip = Array.isArray(ip) ? ip[0] : ip
  const location = await c.getLocationFromIp(req.ip)
  c.log('gray', `${req.method} ${req.path} ${ip}`, location)
  if (location.status === 'success') req.location = location
  next()
})

routes.post('/', async (req, res) => {
  const { score } = req.body
  let scoreAsNumber: number
  try {
    scoreAsNumber = parseInt(score)
  } catch (e) {
    c.log('red', `score is not a number:`, score)
    res.status(400).end()
    return
  }
  if (
    !scoreAsNumber ||
    isNaN(scoreAsNumber) ||
    scoreAsNumber < 0
  ) {
    c.log('red', `score is not a positive number:`, score)
    res.status(400).end()
    return
  }

  c.log('gray', `new score:`, scoreAsNumber)
  const rankings = await db.addScore(
    req.ip,
    req.location,
    scoreAsNumber,
  )
  c.log('gray', `rankings:`, rankings)
  res.json(rankings)
})
export default routes
