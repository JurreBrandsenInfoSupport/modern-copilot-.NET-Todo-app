import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getComments, createComment } from '../api/comments'
import { useAuth } from '../context/AuthContext'
import { MessageSquare, Send, Loader2 } from 'lucide-react'

interface CommentsPanelProps {
  taskId: string
  onClose: () => void
}

export default function CommentsPanel({ taskId, onClose }: CommentsPanelProps) {
  const [text, setText] = useState('')
  const { username } = useAuth()
  const queryClient = useQueryClient()

  const { data: comments = [], isLoading } = useQuery({
    queryKey: ['comments', taskId],
    queryFn: () => getComments(taskId),
  })

  const mutation = useMutation({
    mutationFn: () => createComment(taskId, username || '', text),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['comments', taskId] })
      setText('')
    },
  })

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-2xl w-full max-w-lg mx-4 max-h-[80vh] flex flex-col">
        <div className="flex items-center justify-between p-4 border-b">
          <div className="flex items-center gap-2">
            <MessageSquare size={20} className="text-blue-600" />
            <h3 className="font-semibold text-lg">Comments</h3>
          </div>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 text-xl leading-none"
          >
            ×
          </button>
        </div>

        <div className="flex-1 overflow-auto p-4 space-y-3">
          {isLoading ? (
            <div className="flex justify-center py-8">
              <Loader2 className="animate-spin text-blue-600" size={24} />
            </div>
          ) : comments.length === 0 ? (
            <p className="text-center text-gray-500 py-8">No comments yet</p>
          ) : (
            comments.map((comment) => (
              <div key={comment.id} className="bg-gray-50 rounded-lg p-3">
                <div className="flex items-center justify-between mb-1">
                  <span className="text-sm font-medium text-gray-700">{comment.userId}</span>
                  <span className="text-xs text-gray-400">
                    {new Date(comment.createdAt).toLocaleString()}
                  </span>
                </div>
                <p className="text-sm text-gray-600">{comment.text}</p>
              </div>
            ))
          )}
        </div>

        <div className="p-4 border-t">
          <form
            onSubmit={(e) => {
              e.preventDefault()
              if (text.trim()) mutation.mutate()
            }}
            className="flex gap-2"
          >
            <input
              type="text"
              value={text}
              onChange={(e) => setText(e.target.value)}
              placeholder="Write a comment..."
              className="flex-1 px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <button
              type="submit"
              disabled={!text.trim() || mutation.isPending}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              <Send size={16} />
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}
