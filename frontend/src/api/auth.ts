import axios from 'axios'

export interface TokenResponse {
  token: string
  expiresAt: string
}

export async function getToken(username: string, password?: string): Promise<TokenResponse> {
  const response = await axios.post('/api/auth/token', {
    username,
    password: password ?? 'demo'
  })
  return response.data
}
