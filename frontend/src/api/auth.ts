import axios from 'axios'

export interface TokenResponse {
  token: string
  expiresAt: string
}

export async function getToken(username: string): Promise<TokenResponse> {
  const response = await axios.post('/api/auth/token', { username })
  return response.data
}
