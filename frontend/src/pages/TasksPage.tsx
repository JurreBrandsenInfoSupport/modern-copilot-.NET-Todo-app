import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getTasks, createTask, completeTask } from '../api/tasks'
import { getUsers } from '../api/users'
import CommentsPanel from '../components/CommentsPanel'
import { Plus, CheckCircle2, Circle, MessageSquare, Loader2, Filter } from 'lucide-react'

export default function TasksPage() {
  const [title, setTitle] = useState('')
  const [selectedUserId, setSelectedUserId] = useState('')
  const [filterUserId, setFilterUserId] = useState('')
  const [activeTaskId, setActiveTaskId] = useState<string | null>(null)
  const queryClient = useQueryClient()

  const { data: tasks = [], isLoading } = useQuery({
    queryKey: ['tasks', filterUserId],
    queryFn: () => getTasks(filterUserId || undefined),
  })

  const { data: users = [] } = useQuery({ queryKey: ['users'], queryFn: getUsers })

  const createMutation = useMutation({
    mutationFn: () => createTask(title, selectedUserId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
      setTitle('')
      setSelectedUserId('')
    },
  })

  const completeMutation = useMutation({
    mutationFn: completeTask,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
    },
  })

  return (
    <div>
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Tasks</h2>

      {/* Create task form */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 mb-6">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Create New Task</h3>
        <form
          onSubmit={(e) => {
            e.preventDefault()
            if (title.trim() && selectedUserId) createMutation.mutate()
          }}
          className="flex flex-wrap gap-3"
        >
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Task title"
            className="flex-1 min-w-[200px] px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <select
            value={selectedUserId}
            onChange={(e) => setSelectedUserId(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">Select user</option>
            {users.map((u) => (
              <option key={u.id} value={u.id}>{u.username}</option>
            ))}
          </select>
          <button
            type="submit"
            disabled={!title.trim() || !selectedUserId || createMutation.isPending}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            <Plus size={18} />
            Add Task
          </button>
        </form>
      </div>

      {/* Filter */}
      <div className="flex items-center gap-3 mb-4">
        <Filter size={18} className="text-gray-500" />
        <select
          value={filterUserId}
          onChange={(e) => setFilterUserId(e.target.value)}
          className="px-3 py-1.5 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="">All users</option>
          {users.map((u) => (
            <option key={u.id} value={u.id}>{u.username}</option>
          ))}
        </select>
      </div>

      {/* Task list */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center py-12">
            <Loader2 className="animate-spin text-blue-600" size={32} />
          </div>
        ) : tasks.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <p className="text-lg">No tasks found</p>
            <p className="text-sm mt-1">Create a new task to get started</p>
          </div>
        ) : (
          <div className="divide-y divide-gray-100">
            {tasks.map((task) => (
              <div key={task.id} className="flex items-center justify-between p-4 hover:bg-gray-50 transition-colors">
                <div className="flex items-center gap-3">
                  {task.isCompleted ? (
                    <CheckCircle2 size={22} className="text-green-500" />
                  ) : (
                    <button
                      onClick={() => completeMutation.mutate(task.id)}
                      className="text-gray-300 hover:text-green-500 transition-colors"
                    >
                      <Circle size={22} />
                    </button>
                  )}
                  <div>
                    <p className={`font-medium ${task.isCompleted ? 'line-through text-gray-400' : 'text-gray-800'}`}>
                      {task.title}
                    </p>
                    <p className="text-xs text-gray-400 mt-0.5">
                      {new Date(task.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-3">
                  <span className={`text-xs px-2.5 py-1 rounded-full font-medium ${
                    task.isCompleted
                      ? 'bg-green-100 text-green-700'
                      : 'bg-amber-100 text-amber-700'
                  }`}>
                    {task.isCompleted ? 'Completed' : 'Pending'}
                  </span>
                  <button
                    onClick={() => setActiveTaskId(task.id)}
                    className="p-2 text-gray-400 hover:text-blue-600 rounded-lg hover:bg-blue-50 transition-colors"
                  >
                    <MessageSquare size={18} />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {activeTaskId && (
        <CommentsPanel taskId={activeTaskId} onClose={() => setActiveTaskId(null)} />
      )}
    </div>
  )
}
