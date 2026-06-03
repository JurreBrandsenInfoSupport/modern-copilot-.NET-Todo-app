import { useQuery } from '@tanstack/react-query'
import { getTasks } from '../api/tasks'
import { getUsers } from '../api/users'
import axios from 'axios'
import { ListTodo, Users, HeartPulse, CheckCircle2, Loader2 } from 'lucide-react'

export default function DashboardPage() {
  const { data: tasks = [] } = useQuery({ queryKey: ['tasks'], queryFn: () => getTasks() })
  const { data: users = [] } = useQuery({ queryKey: ['users'], queryFn: getUsers })
  const { data: health } = useQuery({
    queryKey: ['health'],
    queryFn: async () => {
      const res = await axios.get('/health')
      return res.status === 200 ? 'Healthy' : 'Unhealthy'
    },
    retry: false,
  })

  const completedTasks = tasks.filter((t) => t.isCompleted).length
  const pendingTasks = tasks.length - completedTasks

  const cards = [
    { label: 'Total Tasks', value: tasks.length, icon: ListTodo, color: 'bg-blue-500' },
    { label: 'Completed', value: completedTasks, icon: CheckCircle2, color: 'bg-green-500' },
    { label: 'Pending', value: pendingTasks, icon: Loader2, color: 'bg-amber-500' },
    { label: 'Users', value: users.length, icon: Users, color: 'bg-purple-500' },
  ]

  return (
    <div>
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Dashboard</h2>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {cards.map(({ label, value, icon: Icon, color }) => (
          <div key={label} className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">{label}</p>
                <p className="text-3xl font-bold text-gray-800 mt-1">{value}</p>
              </div>
              <div className={`${color} p-3 rounded-lg`}>
                <Icon size={24} className="text-white" />
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
        <div className="flex items-center gap-3 mb-4">
          <HeartPulse size={24} className="text-gray-700" />
          <h3 className="text-lg font-semibold text-gray-800">System Health</h3>
        </div>
        <div className="flex items-center gap-2">
          <div className={`w-3 h-3 rounded-full ${health === 'Healthy' ? 'bg-green-500' : 'bg-red-500'}`} />
          <span className={`font-medium ${health === 'Healthy' ? 'text-green-700' : 'text-red-700'}`}>
            {health || 'Checking...'}
          </span>
        </div>
      </div>
    </div>
  )
}
