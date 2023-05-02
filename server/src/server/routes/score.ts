import * as c from '../../common'
import { Router } from 'express'
import db from '../../db'

const routes = Router()

routes.use(async (req, res, next) => {
  const ip = req.headers['x-forwarded-for'] || req.ip
  req.parsedIp = Array.isArray(ip) ? ip[0] : ip
  const location = await c.getLocationFromIp(req.parsedIp)
  // c.log('gray', `${req.method} ${req.path} ${req.parsedIp}`)
  if (location.status === 'success') {
    req.location = location
    c.log(
      'gray',
      `${req.method} ${req.path} ${req.parsedIp}`,
      location.country,
      location.regionName,
    )
  }
  next()
})

routes.post('/add', async (req, res) => {
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
    scoreAsNumber === undefined ||
    typeof scoreAsNumber !== 'number' ||
    isNaN(scoreAsNumber) ||
    scoreAsNumber < 0
  ) {
    c.log('red', `score is not a positive number:`, score)
    res.status(400).end()
    return
  }

  c.log('gray', `new score:`, scoreAsNumber)
  const rankings = await db.addScore(
    req.parsedIp,
    req.location,
    scoreAsNumber,
  )
  c.log('gray', `rankings:`, rankings)
  res.json(rankings)
})
export default routes
