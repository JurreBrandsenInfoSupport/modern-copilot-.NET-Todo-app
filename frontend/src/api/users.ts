import client from './client'

export interface User {
  id: number
  username: string
}

export async function getUsers(): Promise<User[]> {
  const response = await client.get('/v1/users')
  return response.data
}

export async function createUser(username: string): Promise<User> {
  const response = await client.post('/v1/users', { username })
  return response.data
}
