import axios from 'axios'
// getLocationFromIp.ts
export async function getLocationFromIp(ip: string) {
  const url = `http://ip-api.com/json/${ip}`
  const { data } = await axios.get(url)
  return data
}
