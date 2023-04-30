import fs from 'fs'
import path from 'path'
import * as c from '../common'

/*
 world files are stored in files named `${(thousands place of score)} - ${(number of entries inside)}.json`
  with file names like: `1000 - 20.json`
*/
export default async function (
  dbPath: string,
  ip?: string,
  score?: number,
): Promise<number> {
  if (!ip || !score || dbPath.includes('undefined'))
    return 0

  if (!fs.existsSync(path.join('./', 'data', dbPath))) {
    fs.mkdirSync(path.join('./', 'data', dbPath), {
      recursive: true,
    })
    c.log(`Created ./data/${dbPath} directory`)
  }

  const scoreDataToSave: Score = {
    ip,
    score,
  }
  // get files in world directory
  const files = (
    await fs.promises
      .readdir(path.join('./', 'data', dbPath))
      .catch((err) => {
        c.log(err)
        return []
      })
  ).sort(
    // low to high
    (a, b) =>
      parseInt(a.split(' - ')[0]) -
      parseInt(b.split(' - ')[0]),
  )
  const scoreRoundedToThousands =
    Math.floor(score / 1000) * 1000
  const filesBelowScore = files.filter(
    (file) =>
      parseInt(file.split(' - ')[0]) <
      scoreRoundedToThousands,
  )

  let scoresAboveInFile = 0
  let scoreFile = files.find(
    (file) =>
      parseInt(file.split(' - ')[0]) ===
      scoreRoundedToThousands,
  )
  if (!scoreFile) {
    c.log(
      'creating new score file',
      dbPath + '/' + `${scoreRoundedToThousands} - 1.json`,
    )
    scoreFile = `${scoreRoundedToThousands} - 1.json`
    await fs.promises
      .writeFile(
        path.join('./', 'data', dbPath, scoreFile),
        JSON.stringify([scoreDataToSave]),
      )
      .catch((err) => {
        c.log(err)
      })
  } else {
    c.log(
      'adding score to existing file',
      dbPath + '/' + scoreFile,
    )
    const scoreFileData = JSON.parse(
      (await fs.promises
        .readFile(
          path.join('./', 'data', dbPath, scoreFile),
          'utf8',
        )
        .catch((err) => {
          c.log(err)
          return '[]'
        })) || '[]',
    )
    let scoresAboveInFile = scoreFileData.findIndex(
      (scoreData: Score) => scoreData.score < score,
    )
    if (scoresAboveInFile === -1)
      scoresAboveInFile = scoreFileData.length
    scoreFileData.splice(
      scoresAboveInFile,
      0,
      scoreDataToSave,
    )
    await fs.promises
      .writeFile(
        path.join(
          './',
          'data',
          dbPath,
          `${scoreRoundedToThousands} - ${scoreFileData.length}.json`,
        ),
        JSON.stringify(scoreFileData),
      )
      .catch((err) => {
        c.log(err)
      })
    await fs.promises
      .unlink(path.join('./', 'data', dbPath, scoreFile))
      .catch((err) => {
        c.log(err)
      })
  }

  const scoresInPreviousFiles = filesBelowScore.reduce(
    (total, file) =>
      total + parseInt(file.split(' - ')[1].split('.')[0]),
    0,
  )

  return scoresInPreviousFiles + scoresAboveInFile + 1
}
