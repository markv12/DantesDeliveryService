import * as c from '../../common'
import { Router } from 'express'
import { serverRunningSince } from '..'

const routes = Router()

// routes.use((req, res, next) => {
//   c.log('gray', `${req.method} ${req.path}`)
//   next()
// })

routes.get('/stats', async (req, res) => {
  //   const lastStatEntry =
  //     (await db.stats.getLatest()) || ({} as any)
  //   const dbFilesCount =
  //     lastStatEntry.dbFilesCount || 'unknown'
  //   const entriesPerDay = Math.floor(
  //     (1000 * 60 * 60 * 24) /
  //       (lastStatEntry.span || 1000 * 60 * 60 * 3),
  //   )
  //   const lastEntries = await db.stats.get(entriesPerDay)
  //   const playerCountInLastDay =
  //     c.r2(
  //       lastEntries.reduce(
  //         (acc, cur) => acc + cur.activePlayers,
  //         0,
  //       ),
  //       0,
  //     ) *
  //     (entriesPerDay / lastEntries.length)
  //   const stats = {
  //     // adjusts to approximate when less than a full interval
  //     currentActivePlayerCount:
  //       currentActivePlayerCount *
  //       (c.statSaveInterval /
  //         Math.min(
  //           Date.now() - serverRunningSince,
  //           c.statSaveInterval,
  //         )),
  //     playerCountInLastDay,
  //     activePlayerTimeoutLength:
  //       c.statSaveInterval / 1000 / 60 / 60 + ' hours',
  //     serverRunningFor:
  //       c.r2(
  //         (Date.now() - serverRunningSince) / 1000 / 60 / 60,
  //         2,
  //       ) + ' hours',
  //     actualFilesCount:
  //       lastStatEntry.actualFilesCount || 'unknown',
  //     dbFilesCount,
  //     fileCountDiscrepancy:
  //       (lastStatEntry.actualFilesCount || 0) -
  //       (dbFilesCount?.total || 0),
  //   }
  //   res.json(stats)
  //   c.log('gray', `admin: stats`)
  // })
  // routes.get('/fd/:id/:pw', async (req, res) => {
  //   const id = req.params.id
  //   const pw = req.params.pw
  //   if (pw !== c.aPw) {
  //     c.error('attempted to access admin with wrong password')
  //     res.status(404)
  //     return
  //   }
  //   db.pieces.removeFromDb(id)
  //   deleteAllVersions(`${id}.png`)
  //   res.send(id)
})

export default routes
