import client from './client'

export interface Task {
  id: string
  title: string
  userId: string
  isCompleted: boolean
  createdAt: string
}

export async function getTasks(userId?: string): Promise<Task[]> {
  const params = userId ? { userId } : {}
  const response = await client.get('/v1/tasks', { params })
  return response.data
}

export async function createTask(title: string, userId: string): Promise<Task> {
  const response = await client.post('/v1/tasks', { title, userId })
  return response.data
}

export async function completeTask(id: string): Promise<void> {
  await client.put(`/v1/tasks/${id}/complete`)
}
