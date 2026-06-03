import client from './client'

export interface Comment {
  id: number
  taskItemId: number
  userId: number
  text: string
  createdAt: string
}

export async function getComments(taskItemId: string): Promise<Comment[]> {
  const response = await client.get('/v1/comments', { params: { taskItemId: parseInt(taskItemId) } })
  return response.data
}

export async function createComment(taskItemId: string, userId: string, text: string): Promise<Comment> {
  const response = await client.post('/v1/comments', { taskItemId: parseInt(taskItemId), userId: parseInt(userId), text })
  return response.data
}
