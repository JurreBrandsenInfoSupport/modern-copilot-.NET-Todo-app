import client from './client'

export interface Comment {
  id: string
  taskItemId: string
  userId: string
  text: string
  createdAt: string
}

export async function getComments(taskItemId: string): Promise<Comment[]> {
  const response = await client.get('/v1/comments', { params: { taskItemId } })
  return response.data
}

export async function createComment(taskItemId: string, userId: string, text: string): Promise<Comment> {
  const response = await client.post('/v1/comments', { taskItemId, userId, text })
  return response.data
}
