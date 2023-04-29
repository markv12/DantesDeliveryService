import { log } from './log'

export const steamAppId = '2102020'

export function randomIntBetween(
  min: number,
  max: number,
): number {
  return Math.floor(Math.random() * (max - min + 1)) + min
}

export function shuffle(array: any[]): any[] {
  let currentIndex = array.length,
    randomIndex

  // While there remain elements to shuffle...
  while (currentIndex != 0) {
    // Pick a remaining element...
    randomIndex = Math.floor(Math.random() * currentIndex)
    currentIndex--

    // And swap it with the current element.
    ;[array[currentIndex], array[randomIndex]] = [
      array[randomIndex],
      array[currentIndex],
    ]
  }

  return array
}

export function getId(prefix?: string) {
  return `${prefix || ''}${Date.now()}@${randomIntBetween(
    0,
    1000000,
  )}`
}

// roundTo:
// @param number (number) Initial number
// @param decimalPlaces (number) Number of decimal places to round to
// @param floor? (boolean) If true, uses floor instead of round.
//
export function r2(
  number: number = 0,
  decimalPlaces: number = 2,
  floorOrCeiling?: 'floor' | 'ceiling',
): number {
  if (floorOrCeiling === 'floor')
    return (
      Math.floor(number * 10 ** decimalPlaces) /
      10 ** decimalPlaces
    )
  else if (floorOrCeiling === 'ceiling')
    return (
      Math.ceil(number * 10 ** decimalPlaces) /
      10 ** decimalPlaces
    )
  return (
    Math.round(number * 10 ** decimalPlaces) /
    10 ** decimalPlaces
  )
}

export function sleep(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms))
}

export function hexColorDifferencePercent(
  c1: string,
  c2: string,
): number {
  const hsl2 = hexToHsl(c2)
  const hsl1 = hexToHsl(c1)
  return hslColorDifferencePercent(hsl1, hsl2)
}
export function hslColorDifferencePercent(
  hsl1: { h: number; s: number; l: number },
  hsl2: { h: number; s: number; l: number },
): number {
  const hDiff = Math.abs(hsl2.h - hsl1.h)
  const hDiffPercent = hDiff / 360
  const sDiff = Math.abs(hsl2.s - hsl1.s)
  const sDiffPercent = sDiff / 100
  const lDiff = Math.abs(hsl2.l - hsl1.l)
  const lDiffPercent = lDiff / 100
  return r2(
    ((hDiffPercent + sDiffPercent + lDiffPercent) / 3) *
      100,
    2,
  )
}

export function hexToHsl(rgb: string) {
  rgb = rgb.replace('#', '').toLowerCase()
  const r = parseInt(rgb.substring(0, 2), 16)
  const g = parseInt(rgb.substring(2, 4), 16)
  const b = parseInt(rgb.substring(4, 6), 16)

  const max = Math.max(r, g, b)
  const min = Math.min(r, g, b)
  const l = (max + min) / 2

  if (max === min) {
    return { h: 0, s: 0, l }
  }

  let h
  let s

  if (max === r) {
    h = (g - b) / (max - min)
  } else if (max === g) {
    h = 2 + (b - r) / (max - min)
  } else {
    h = 4 + (r - g) / (max - min)
  }

  h = h * 60
  if (h < 0) {
    h = h + 360
  }

  const chroma = max - min
  s = chroma / (1 - Math.abs(2 * l - 1))

  return { h, s, l }
}
export function hslToHex(hsl: {
  h: number
  s: number
  l: number
}): string {
  let h = hsl.h * 360
  let s = hsl.s
  let l = hsl.l

  let c = (1 - Math.abs(2 * l - 1)) * s,
    x = c * (1 - Math.abs(((h / 60) % 2) - 1)),
    m = l - c / 2,
    r: any = 0,
    g: any = 0,
    b: any = 0

  if (0 <= h && h < 60) {
    r = c
    g = x
    b = 0
  } else if (60 <= h && h < 120) {
    r = x
    g = c
    b = 0
  } else if (120 <= h && h < 180) {
    r = 0
    g = c
    b = x
  } else if (180 <= h && h < 240) {
    r = 0
    g = x
    b = c
  } else if (240 <= h && h < 300) {
    r = x
    g = 0
    b = c
  } else if (300 <= h && h < 360) {
    r = c
    g = 0
    b = x
  }
  // Having obtained RGB, convert channels to hex
  r = Math.round((r + m) * 255).toString(16)
  g = Math.round((g + m) * 255).toString(16)
  b = Math.round((b + m) * 255).toString(16)

  // Prepend 0s, if necessary
  if (r.length == 1) r = '0' + r
  if (g.length == 1) g = '0' + g
  if (b.length == 1) b = '0' + b

  return '#' + r + g + b
}

export function avg(...nums: number[]): number {
  return nums.reduce((a, b) => a + b, 0) / nums.length
}

export function getDataFromUrlEncodedBody(b: Buffer) {
  const asString = b.toString()
  const data: any = {}
  for (let pair of asString.split('&')) {
    const [key, value] = pair.split('=')
    data[decodeURIComponent(key)] =
      decodeURIComponent(value)
  }
  return data
}

export const memoed: { [key: string]: any } = {}
const expired: { [key: string]: boolean | 'fetching' } = {}
const memoKeyTimeouts: { [key: string]: NodeJS.Timeout } =
  {}
export async function getMemoed(
  key: any,
  fallback: Function,
  expireTime?: number,
) {
  if (memoed[key]) {
    if (expired[key] === true) {
      refreshMemoedValue(key, fallback)
    }
    return memoed[key]
  }
  // initial get
  const value = await fallback()
  memoValue(key, value, expireTime)
  return value
}
async function refreshMemoedValue(
  key: any,
  refreshFunction: Function,
  expireTime?: number,
) {
  expired[key] = 'fetching'
  const newValue = await refreshFunction()
  memoValue(key, newValue, expireTime)
}
function memoValue(
  key: any,
  value: any,
  expireTime: number = 5000,
  fetchFunction?: Function,
) {
  memoed[key] = value
  if (memoKeyTimeouts[key])
    clearTimeout(memoKeyTimeouts[key])
  memoKeyTimeouts[key] = setTimeout(() => {
    expired[key] = true
  }, expireTime)
}
