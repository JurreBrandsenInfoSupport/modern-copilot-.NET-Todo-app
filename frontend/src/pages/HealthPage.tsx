import { useQuery } from '@tanstack/react-query'
import axios from 'axios'
import { HeartPulse, CheckCircle2, XCircle, RefreshCw } from 'lucide-react'

export default function HealthPage() {
  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: ['health-detail'],
    queryFn: async () => {
      const res = await axios.get('/health')
      return { status: res.status, data: res.data }
    },
    retry: false,
  })

  const isHealthy = data?.status === 200

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-gray-800">Health Check</h2>
        <button
          onClick={() => refetch()}
          className="flex items-center gap-2 px-4 py-2 text-sm bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
        >
          <RefreshCw size={16} />
          Refresh
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-8">
        <div className="flex flex-col items-center text-center">
          <div className={`p-4 rounded-full mb-4 ${
            isLoading ? 'bg-gray-100' : isHealthy ? 'bg-green-100' : 'bg-red-100'
          }`}>
            {isLoading ? (
              <HeartPulse size={48} className="text-gray-400 animate-pulse" />
            ) : isHealthy ? (
              <CheckCircle2 size={48} className="text-green-500" />
            ) : (
              <XCircle size={48} className="text-red-500" />
            )}
          </div>

          <h3 className={`text-2xl font-bold mb-2 ${
            isLoading ? 'text-gray-500' : isHealthy ? 'text-green-700' : 'text-red-700'
          }`}>
            {isLoading ? 'Checking...' : isHealthy ? 'System Healthy' : 'System Unhealthy'}
          </h3>

          <p className="text-gray-500 max-w-md">
            {isLoading
              ? 'Running health check against the API...'
              : isHealthy
                ? 'All systems are operational. The API is responding normally.'
                : isError
                  ? 'Unable to reach the API. Please check that the backend is running.'
                  : 'The API returned an unexpected status.'}
          </p>

          {data && (
            <div className="mt-6 w-full max-w-sm">
              <div className="bg-gray-50 rounded-lg p-4 text-left">
                <p className="text-sm text-gray-500 mb-1">Endpoint</p>
                <p className="text-sm font-mono text-gray-700">/health</p>
                <p className="text-sm text-gray-500 mb-1 mt-3">Status Code</p>
                <p className="text-sm font-mono text-gray-700">{data.status}</p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
