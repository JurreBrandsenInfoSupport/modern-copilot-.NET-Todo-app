import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getUsers, createUser } from '../api/users'
import { UserPlus, Users as UsersIcon, Loader2 } from 'lucide-react'

export default function UsersPage() {
  const [username, setUsername] = useState('')
  const queryClient = useQueryClient()

  const { data: users = [], isLoading } = useQuery({ queryKey: ['users'], queryFn: getUsers })

  const mutation = useMutation({
    mutationFn: () => createUser(username),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] })
      setUsername('')
    },
  })

  return (
    <div>
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Users</h2>

      {/* Create user form */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 mb-6">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Register New User</h3>
        <form
          onSubmit={(e) => {
            e.preventDefault()
            if (username.trim()) mutation.mutate()
          }}
          className="flex gap-3"
        >
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Username"
            className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <button
            type="submit"
            disabled={!username.trim() || mutation.isPending}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            <UserPlus size={18} />
            Register
          </button>
        </form>
      </div>

      {/* User list */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="flex items-center gap-2 p-4 border-b border-gray-100">
          <UsersIcon size={20} className="text-gray-600" />
          <h3 className="font-semibold text-gray-800">All Users ({users.length})</h3>
        </div>
        {isLoading ? (
          <div className="flex justify-center py-12">
            <Loader2 className="animate-spin text-blue-600" size={32} />
          </div>
        ) : users.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <p>No users registered yet</p>
          </div>
        ) : (
          <div className="divide-y divide-gray-100">
            {users.map((user) => (
              <div key={user.id} className="flex items-center gap-3 p-4">
                <div className="w-10 h-10 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center font-semibold text-sm">
                  {user.username.charAt(0).toUpperCase()}
                </div>
                <div>
                  <p className="font-medium text-gray-800">{user.username}</p>
                  <p className="text-xs text-gray-400">ID: {user.id}</p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
