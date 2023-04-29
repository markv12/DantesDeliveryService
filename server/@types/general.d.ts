type ResponseOrError<T> = T | { error: any }

interface FileDataPreDb {
  id: string
  iteration: number
  created: number
  updated: number
  likes: number
  dislikes: number
  collections?: string[]
  creator1Username?: string | null
  creator2Username?: string | null
  authorMaxLikes?: number
}

interface FileDataForDb extends FileDataPreDb {
  likesMinusDislikes: number
  ratio: number
  ranking: number
  shadow?: boolean /** shadow-buried */
}

interface FileDataForFrontend {
  id: string
  path: string
  originalPath: string
  creator1Username?: string
  creator2Username?: string
  authorMaxLikes?: number
  updated?: number
  created?: number
  likes?: number
  // couldBeNsfw?: true
}

interface GameStat {
  activePlayers: number
  time: number
  span: number
  actualFilesCount
  dbFilesCount: {
    total: number
    iteration1: number
    iteration2: number
  }
}

interface ServerTokenData {
  token: string
  id: string
  /** added to timestamp */
  tsMod: number
  used: number[]
  validUntil: number
}

interface CollectionData {
  id: string
  weekly: boolean
  start: number
  end: number
}

interface SignUpUserData {
  username?: string
  email?: string
  password?: string
}
interface SteamSignUpUserData {
  username?: string
  steamUserTicket?: string
}
interface UserData
  extends Omit<SignUpUserData, 'rawPassword'> {
  firstSeenIp: string
  lastSeenIp: string
  id: string
  hashedPassword?: string
  steamId?: string
  pieceIdsIteration1: string[]
  pieceIdsIteration2: string[]
  isDeluxe: boolean
  lastSeen: number
  created: number
  feed: FeedEntry[]
  lastCheckedFeed: number
  draft: boolean
  draftPaintingId?: string | null
  likedPieceIds: string[]
  banned?: boolean
  shadow?: boolean /** shadowbanned */
  maxLikes?: number
}
interface FeedEntry {
  date: number
  type: 'like'
  pieceId?: string
  count?: number
}
interface FeedEntryForFrontend extends FeedEntry {
  path: string
}

interface UserDataForFrontend {
  username?: string
  email?: string
  id: string
  myPieces: FileDataForFrontend[]
  isDeluxe: boolean
  feed: FeedEntryForFrontend[]
  lastCheckedFeed: number
  likedPieces: FileDataForFrontend[]
  draft: string
  draftPaintingId?: string
  maxLikes: Number
}
